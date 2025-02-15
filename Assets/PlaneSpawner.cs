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
    // מרחק ההופעה מהרצפה – כלומר, המרחק שממנו מופיע חלק חדש של הרצפה
    public float spawnDistance = 5f;
    // מרחק המחיקה – המרחק מהשחקן שבו האובייקט הקודם יימחק
    public float deletionDistance = 10f;

    public List<SpawnableObject> spawnableObjects;
    public Transform player;

    // רשימה לשמירת חלקי הרצפה שהופיעו
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
        // יצירת רצפה חדשה כאשר השחקן התקדם מרחק מסוים
        if (Vector3.Distance(player.position, lastSpawnPosition) >= spawnDistance)
        {
            SpawnObject();
            lastSpawnPosition = player.position;
        }

        // בדיקה ומחיקת חלקי רצפה מרוחקים מהשחקן
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
        // נבדוק כל חלק ברשימה אם הוא רחוק מהשחקן
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
                // במידה והאובייקט כבר נעלם מסיבה אחרת
                spawnedFloors.RemoveAt(i);
            }
        }
    }
}
