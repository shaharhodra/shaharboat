using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class FloorPrefab
{
    public GameObject prefab;
    public float weight = 1f;
}

public class FloorManager : MonoBehaviour
{
    [Header("������ ����� �������")]
    public Transform player;                // ����� �����
    public List<FloorPrefab> floorPrefabs;  // ����� �� ������ �� ������
    public float tileSize = 10f;            // ���� �� ����

    // �� ����� ����� (Grid) ������ �� ����� ��� �����
    private Vector2Int currentCenter;
    // ����� ������ �� �� ������ ������� ����� �� ������ (���� X ��-Z)
    private Dictionary<Vector2Int, GameObject> activeTiles = new Dictionary<Vector2Int, GameObject>();

    void Start()
    {
        // ����� �� ����� ������ ������ �� ����� �����
        currentCenter = new Vector2Int(
            Mathf.RoundToInt(player.position.x / tileSize),
            Mathf.RoundToInt(player.position.z / tileSize)
        );
        UpdateTiles();
    }

    void Update()
    {
        // ��� �� ��� ��� ����� ����� ������
        Vector2Int newCenter = new Vector2Int(
            Mathf.RoundToInt(player.position.x / tileSize),
            Mathf.RoundToInt(player.position.z / tileSize)
        );

        // �� ����� ��� ��� ���, ���� �� ������
        if (newCenter != currentCenter)
        {
            currentCenter = newCenter;
            UpdateTiles();
        }
    }

    /// <summary>
    /// ������� �� ����� ������ ���� ���� ������ ������ �� ������� �������
    /// </summary>
    /// <returns>������� ���� ����</returns>
    GameObject GetRandomFloorPrefab()
    {
        // ����� �� �� �������
        float totalWeight = 0f;
        foreach (FloorPrefab fp in floorPrefabs)
        {
            totalWeight += fp.weight;
        }

        // ������ ���� ������� ��� 0 ��totalWeight
        float randomValue = Random.Range(0, totalWeight);

        // ���� �� ������ �� ������ ���� �� ���� ��������
        foreach (FloorPrefab fp in floorPrefabs)
        {
            randomValue -= fp.weight;
            if (randomValue <= 0)
            {
                return fp.prefab;
            }
        }

        // ����� ����, ������� �� ����� ������
        return floorPrefabs[0].prefab;
    }

    /// <summary>
    /// ����� ��� ������� �� ����� ���� ���� 9 ����� (���� 3�3)
    /// </summary>
    void UpdateTiles()
    {
        // ����� ���� ������ ������ ��� ������
        Dictionary<Vector2Int, GameObject> newTiles = new Dictionary<Vector2Int, GameObject>();

        // ���� �� �� ����� 3x3 ���� ����� (���� ����, ����� ������ ����������)
        for (int offsetX = -1; offsetX <= 1; offsetX++)
        {
            for (int offsetZ = -1; offsetZ <= 1; offsetZ++)
            {
                Vector2Int tileCoord = new Vector2Int(currentCenter.x + offsetX, currentCenter.y + offsetZ);

                // �� ����� ����� ��� ������ ���, ������� ���� ������ ����
                if (activeTiles.ContainsKey(tileCoord))
                {
                    newTiles[tileCoord] = activeTiles[tileCoord];
                    activeTiles.Remove(tileCoord);
                }
                else
                {
                    // ����, ���� ������� ���� ���
                    Vector3 spawnPos = new Vector3(tileCoord.x * tileSize, 0, tileCoord.y * tileSize);
                    // ����� ���� ������ �� ������� �������
                    GameObject tilePrefab = GetRandomFloorPrefab();
                    GameObject newTile = Instantiate(tilePrefab, spawnPos, Quaternion.identity);
                    newTiles[tileCoord] = newTile;
                }
            }
        }

        // ����� ������ ��� ������ ����� 3x3 ����
        foreach (KeyValuePair<Vector2Int, GameObject> tile in activeTiles)
        {
            Destroy(tile.Value);
        }

        // ����� ������ �� ������ �������
        activeTiles = newTiles;
    }
}
