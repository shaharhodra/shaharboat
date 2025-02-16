using UnityEngine;
using System.Collections.Generic;

public class RandomObjectPlacer : MonoBehaviour
{
    public GameObject[] objectsToPlace; // ���� �� ��������� �����
    public int numberOfObjectsToPlace = 10; // ���� ���������� �����
    public float maxDistanceFromPlayer = 5f; // ���� ������� ������ ����� ����������
    public float minDistanceFromPlayer = 2f; // ���� ������� ������ ����� ����������
    public float minDistanceBetweenObjects = 1f; // ���� ������� ��� ����������
    public float maxDistanceFromPlayerToDisappear = 10f; // ���� ������� ������ ���� ��������� �����

    // �� ����� ����� ��� ��� ����� ���������
    public string forbiddenTag = "shore";

    private List<GameObject> placedObjects = new List<GameObject>(); // ����� ����� ���� ���������� ������
    private List<GameObject> availableObjects = new List<GameObject>(); // ����� ���������� ������ �����

    void Start()
    {
        // ����� ������ �� ���������� �������
        availableObjects = new List<GameObject>(objectsToPlace);
        PlaceObjectsRandomly();
    }

    void Update()
    {
        // ����� ��� ��������� ������ ��� ������ �� ���� ���� �����, �������
        CheckAndReplaceObjects();
    }

    void PlaceObjectsRandomly()
    {
        if (objectsToPlace.Length == 0) return;

        // ���� ���� ���������� �����
        while (placedObjects.Count < numberOfObjectsToPlace)
        {
            // ���� ������� ������ �����
            GameObject objectToPlace = GetUniqueObject();

            // ����� ����� ����� ���� ����� ����� ������
            Vector3 randomPosition = GetRandomPositionAroundPlayer();

            // ����� ��� ������ ���� (�� ���� ��� ���������� ����� ��� �� ����� ����)
            if (IsPositionValid(randomPosition) && !IsPositionInForbiddenArea(randomPosition))
            {
                // ����� �������� ������ ������
                GameObject placedObject = Instantiate(objectToPlace, randomPosition, Quaternion.identity);

                // ��� ����� ����� ��������
                placedObject.transform.Rotate(0, Random.Range(0f, 360f), 0);

                // ����� �������� ������ ���������� ������
                placedObjects.Add(placedObject);
            }
        }
    }

    // ������� ����� ������� ������ (����� ������ �������)
    GameObject GetUniqueObject()
    {
        if (availableObjects.Count == 0)
        {
            availableObjects = new List<GameObject>(objectsToPlace);
        }

        GameObject uniqueObject = availableObjects[Random.Range(0, availableObjects.Count)];
        availableObjects.Remove(uniqueObject);

        return uniqueObject;
    }

    // ������� ����� ����� ����� ����� (������� ��������) ������
    Vector3 GetRandomPositionAroundPlayer()
    {
        float distance = Random.Range(minDistanceFromPlayer, maxDistanceFromPlayer);
        float angle = Random.Range(0f, Mathf.PI * 2);
        float x = Mathf.Cos(angle) * distance;
        float z = Mathf.Sin(angle) * distance;
        return new Vector3(x, 0, z) + transform.position;
    }

    // ������� ������ ������ ������ (���� �� ���� ��� ���������� �����)
    bool IsPositionValid(Vector3 position)
    {
        foreach (GameObject placedObject in placedObjects)
        {
            if (Vector3.Distance(position, placedObject.transform.position) < minDistanceBetweenObjects)
            {
                return false;
            }
        }
        return true;
    }

    // ������� ������ �� ������ ���� ���� ��� �������� �� ��� ����� ("dint")
    bool IsPositionInForbiddenArea(Vector3 position)
    {
        GameObject[] forbiddenAreas = GameObject.FindGameObjectsWithTag(forbiddenTag);
        foreach (GameObject forbiddenArea in forbiddenAreas)
        {
            Collider col = forbiddenArea.GetComponent<Collider>();
            if (col != null && col.bounds.Contains(position))
            {
                return true;
            }
        }
        return false;
    }

    // ������� ������ �� ���������� ������ ��� ������ �� ������ ����� �����, ������� ����
    void CheckAndReplaceObjects()
    {
        for (int i = placedObjects.Count - 1; i >= 0; i--)
        {
            GameObject placedObject = placedObjects[i];
            float distanceFromPlayer = Vector3.Distance(placedObject.transform.position, transform.position);
            bool isInForbiddenArea = IsPositionInForbiddenArea(placedObject.transform.position);

            if (distanceFromPlayer > maxDistanceFromPlayerToDisappear || isInForbiddenArea)
            {
                Destroy(placedObject);
                GameObject objectToPlace = GetUniqueObject();
                Vector3 newRandomPosition = GetRandomPositionAroundPlayer();

                // �� ������ ���� �� �� ���� �� ���� ����� ����, ���� ���� �����
                int attempts = 0;
                while ((!IsPositionValid(newRandomPosition) || IsPositionInForbiddenArea(newRandomPosition)) && attempts < 10)
                {
                    newRandomPosition = GetRandomPositionAroundPlayer();
                    attempts++;
                }

                GameObject newPlacedObject = Instantiate(objectToPlace, newRandomPosition, Quaternion.identity);
                newPlacedObject.transform.Rotate(0, Random.Range(0f, 360f), 0);
                placedObjects[i] = newPlacedObject;
            }
        }
    }
}
