using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class SpawnableObject
{
    public GameObject prefab;
    public float weight = 1f;
}

public class PlaneSpawner : MonoBehaviour
{
    // ���� ������ ������ � �����, ����� ����� ����� ��� ��� �� �����
    public float spawnDistance = 5f;
    // ���� ������ � ����� ������ ��� �������� ����� �����
    public float deletionDistance = 10f;

    public List<SpawnableObject> spawnableObjects;
    public Transform player;

    // ����� ������ ���� ����� �������
    private List<GameObject> spawnedFloors = new List<GameObject>();
    private Vector3 lastSpawnPosition;

    void Start()
    {
        if (player == null)
        {
            player = transform;
        }
        lastSpawnPosition = player.position;
    }

    void Update()
    {
        // ����� ���� ���� ���� ����� ����� ���� �����
        if (Vector3.Distance(player.position, lastSpawnPosition) >= spawnDistance)
        {
            SpawnObject();
            lastSpawnPosition = player.position;
        }

        // ����� ������ ���� ���� ������� ������
        CheckAndDeleteFloors();
    }

    GameObject ChooseRandomObject()
    {
        float totalWeight = 0f;
        foreach (var item in spawnableObjects)
        {
            totalWeight += item.weight;
        }

        float randomValue = Random.Range(0f, totalWeight);
        float cumulativeWeight = 0f;
        foreach (var item in spawnableObjects)
        {
            cumulativeWeight += item.weight;
            if (randomValue <= cumulativeWeight)
            {
                return item.prefab;
            }
        }
        return spawnableObjects[0].prefab;
    }

    void SpawnObject()
    {
        Vector3 spawnPosition = player.position + player.forward * spawnDistance;
        spawnPosition.y = 0f;

        GameObject prefabToSpawn = ChooseRandomObject();
        GameObject newFloor = Instantiate(prefabToSpawn, spawnPosition, Quaternion.identity);
        spawnedFloors.Add(newFloor);
    }

    void CheckAndDeleteFloors()
    {
        // ����� �� ��� ������ �� ��� ���� ������
        for (int i = spawnedFloors.Count - 1; i >= 0; i--)
        {
            if (spawnedFloors[i] != null)
            {
                float distance = Vector3.Distance(player.position, spawnedFloors[i].transform.position);
                if (distance >= deletionDistance)
                {
                    Destroy(spawnedFloors[i]);
                    spawnedFloors.RemoveAt(i);
                }
            }
            else
            {
                // ����� ��������� ��� ���� ����� ����
                spawnedFloors.RemoveAt(i);
            }
        }
    }
}
