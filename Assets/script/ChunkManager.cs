using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkManager : MonoBehaviour
{
    public Transform player;
    public int chunkSize = 50;
    public int loadRadius = 3;
    public int forwardLoadDistance = 5;
    public int unloadDistance = 5;
    public float chunkLoadDelay = 0.2f;

    [Header("Island & Beach Settings")]
    [Tooltip("מרווח בין צ'אנקים להופעת האיים")]
    public int islandSpacing = 8;
    [Tooltip("מרווח בין צ'אנקים להופעת החופים")]
    public int beachSpacing = 16;

    [Header("Distance Thresholds")]
    [Tooltip("מרחק מינימלי (בצ'אנקים) מהשחקן להופעת איים")]
    public int minDistanceForIsland = 3;
    [Tooltip("מרחק מינימלי (בצ'אנקים) מהשחקן להופעת חופים")]
    public int minDistanceForBeach = 5;

    [System.Serializable]
    public class ChunkType
    {
        public string name;
        public GameObject prefab;
    }

    public List<ChunkType> chunkPrefabs;

    private Dictionary<Vector2, GameObject> loadedChunks = new Dictionary<Vector2, GameObject>();
    private Queue<Vector2> chunkQueue = new Queue<Vector2>();
    private bool isLoadingChunk = false;
    private Vector3 lastPlayerPosition;
    private Vector2 lastPlayerChunkPos;

    void Start()
    {
        lastPlayerPosition = player.position;
        lastPlayerChunkPos = GetChunkCoords(player.position);
        StartCoroutine(LoadChunksGradually());
    }

    void Update()
    {
        Vector2 currentChunkPos = GetChunkCoords(player.position);
        if (currentChunkPos != lastPlayerChunkPos && !isLoadingChunk)
        {
            lastPlayerChunkPos = currentChunkPos;
            StartCoroutine(LoadChunksGradually());
        }
        UnloadFarChunks();
    }

    Vector2 GetChunkCoords(Vector3 position)
    {
        return new Vector2(Mathf.Floor(position.x / chunkSize), Mathf.Floor(position.z / chunkSize));
    }

    IEnumerator LoadChunksGradually()
    {
        isLoadingChunk = true;
        chunkQueue.Clear();

        Vector2 playerChunkPos = lastPlayerChunkPos;

        for (int x = -loadRadius; x <= loadRadius; x++)
        {
            for (int y = -loadRadius; y <= loadRadius + forwardLoadDistance; y++)
            {
                Vector2 chunkCoord = new Vector2(playerChunkPos.x + x, playerChunkPos.y + y);

                // תמיד טעינת הצ'אנק שמתחת לשחקן כים
                if (chunkCoord == playerChunkPos)
                {
                    LoadChunk(chunkCoord, "Chunk_Ocean");
                    continue;
                }

                if (!loadedChunks.ContainsKey(chunkCoord))
                {
                    chunkQueue.Enqueue(chunkCoord);
                }
            }
        }

        while (chunkQueue.Count > 0)
        {
            Vector2 chunkCoord = chunkQueue.Dequeue();
            LoadChunk(chunkCoord);
            yield return new WaitForSeconds(chunkLoadDelay);
        }

        isLoadingChunk = false;
    }

    void LoadChunk(Vector2 chunkCoord, string forcedType = "")
    {
        if (loadedChunks.ContainsKey(chunkCoord))
            return;

        string chunkType = forcedType != "" ? forcedType : DetermineChunkType(chunkCoord);
        GameObject chunkPrefab = GetChunkPrefab(chunkType);

        if (chunkPrefab != null)
        {
            Vector3 chunkPosition = new Vector3(chunkCoord.x * chunkSize, 0, chunkCoord.y * chunkSize);
            GameObject chunkInstance = Instantiate(chunkPrefab, chunkPosition, Quaternion.identity);
            loadedChunks.Add(chunkCoord, chunkInstance);
            Debug.Log($"✅ Loaded Chunk: {chunkType} at {chunkCoord}");
        }
        else
        {
            Debug.LogError($"❌ Chunk Prefab not found for type: {chunkType}");
        }
    }

    void UnloadFarChunks()
    {
        HashSet<Vector2> toRemove = new HashSet<Vector2>();
        Vector2 playerChunkPos = lastPlayerChunkPos;
        float unloadDistanceSquared = unloadDistance * unloadDistance;

        foreach (var chunk in loadedChunks)
        {
            float distanceSquared = (chunk.Key - playerChunkPos).sqrMagnitude;
            if (distanceSquared > unloadDistanceSquared)
            {
                Destroy(chunk.Value);
                toRemove.Add(chunk.Key);
            }
        }

        foreach (var key in toRemove)
        {
            loadedChunks.Remove(key);
        }
    }

    string DetermineChunkType(Vector2 coord)
    {
        // חשב את מרכז הצ'אנק בעולם (לדיוק במרחק מהשחקן)
        Vector3 chunkCenter = new Vector3(coord.x * chunkSize + chunkSize * 0.5f, 0, coord.y * chunkSize + chunkSize * 0.5f);
        Vector3 toChunk = (chunkCenter - player.position).normalized;

        // מחשבים את הכיוון שהשחקן פונה בציר XZ
        Vector3 playerForwardXZ = new Vector3(player.forward.x, 0, player.forward.z).normalized;

        // אם הצ'אנק מאחוריו של השחקן – תמיד יהיה ים
        if (Vector3.Dot(toChunk, playerForwardXZ) < 0)
        {
            return "Chunk_Ocean";
        }

        float distanceFromPlayer = Vector3.Distance(chunkCenter, player.position);

        // אם הצ'אנק קרוב מדי (פחות מ־minDistanceForBeach בצ'אנקים), גם אם עומד בתנאי החוף – יהיה ים
        if (distanceFromPlayer < minDistanceForBeach * chunkSize)
        {
            return "Chunk_Ocean";
        }

        // במידה והצ'אנק רחוק מספיק, נבדוק תנאי לאיים
        if (Mathf.Abs(coord.x) % islandSpacing == 0 && Mathf.Abs(coord.y) % islandSpacing == 0 &&
            distanceFromPlayer >= minDistanceForIsland * chunkSize)
        {
            return "Chunk_Island";
        }

        // בדיקת תנאי לחוף
        if (Mathf.Abs(coord.x) % beachSpacing == 0 && Mathf.Abs(coord.y) % beachSpacing == 0)
        {
            return "Chunk_Beach";
        }

        return "Chunk_Ocean";
    }

    GameObject GetChunkPrefab(string type)
    {
        foreach (var chunk in chunkPrefabs)
        {
            if (chunk.name == type)
                return chunk.prefab;
        }
        return null;
    }
}
