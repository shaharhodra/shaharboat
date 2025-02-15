using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkManager : MonoBehaviour
{
    public Transform player;
    public int chunkSize = 50;
    public int loadRadius = 5;            // כמה צ'אנקים נטענים סביב השחקן
    public int forwardLoadDistance = 3;   // טעינה נוספת קדימה לפי כיוון השחקן
    public int unloadDistance = 7;        // מרחק (בצ'אנקים) שבו צ'אנקים נשאבים
    public float chunkLoadDelay = 0.1f;

    [Header("Island & Beach Settings")]
    [Tooltip("מרווח בין צ'אנקים להופעת האיים")]
    public int islandSpacing = 8;
    [Tooltip("מרווח בין צ'אנקים להופעת החופים")]
    public int beachSpacing = 4;

    [Header("Distance Thresholds")]
    [Tooltip("מרחק מינימלי (בצ'אנקים) מהשחקן להופעת חופים")]
    public int minDistanceForBeach = 2;
    [Tooltip("מרחק מינימלי (בצ'אנקים) מהשחקן להופעת איים")]
    public int minDistanceForIsland = 4;

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
    private Vector2 lastPlayerChunkPos;

    void Start()
    {
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

    // מחשב את הקואורדינטות של הצ'אנק בהתאם למיקום בעולם
    Vector2 GetChunkCoords(Vector3 position)
    {
        return new Vector2(Mathf.Floor(position.x / chunkSize), Mathf.Floor(position.z / chunkSize));
    }

    // טעינה הדרגתית של צ'אנקים מסביב לשחקן
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

                // תמיד טוענים את הצ'אנק שמעליו נמצא השחקן כבסיס – ים
                if (chunkCoord == playerChunkPos)
                {
                    LoadChunk(chunkCoord, "Chunk_Ocean");
                    continue;
                }

                if (!loadedChunks.ContainsKey(chunkCoord))
                    chunkQueue.Enqueue(chunkCoord);
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

    // טוען צ'אנק בודד לפי קואורדינטה, ניתן להכריח סוג מסוים (למשל "Chunk_Ocean")
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
            Debug.Log($"✅ Loaded {chunkType} at {chunkCoord}");
        }
        else
        {
            Debug.LogError($"❌ Chunk Prefab not found for type: {chunkType}");
        }
    }

    // מסיר צ'אנקים רחוקים מדי מהשחקן
    void UnloadFarChunks()
    {
        HashSet<Vector2> toRemove = new HashSet<Vector2>();
        Vector2 playerChunkPos = lastPlayerChunkPos;
        float unloadDistanceSquared = unloadDistance * unloadDistance;

        foreach (var chunk in loadedChunks)
        {
            if ((chunk.Key - playerChunkPos).sqrMagnitude > unloadDistanceSquared)
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

    // קובע את סוג הצ'אנק על פי מרחק מהשחקן ותנאים פשוטים להופעת חופים ואיים
    string DetermineChunkType(Vector2 coord)
    {
        // חשב את מרכז הצ'אנק בעולם
        Vector3 chunkCenter = new Vector3(coord.x * chunkSize + chunkSize * 0.5f, 0, coord.y * chunkSize + chunkSize * 0.5f);
        float distanceFromPlayer = Vector3.Distance(chunkCenter, player.position);

        // אם הצ'אנק קרוב מדי – תמיד ים
        if (distanceFromPlayer < minDistanceForBeach * chunkSize)
            return "Chunk_Ocean";

        // במרחק בינוני – נציג חוף
        if (distanceFromPlayer < minDistanceForIsland * chunkSize)
            return "Chunk_Beach";

        // במרחק רחוק – אם עונה על תנאי המרווח (לפי מודולו) נציג אי
        if (((int)Mathf.Abs(coord.x)) % islandSpacing == 0 && ((int)Mathf.Abs(coord.y)) % islandSpacing == 0)
            return "Chunk_Island";

        // במקרים אחרים – ים
        return "Chunk_Ocean";
    }

    // מחפש את הפרפאב המתאים לפי שם סוג הצ'אנק
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
