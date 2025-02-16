using UnityEngine;
using System.Collections.Generic;

public class RandomObjectPlacer : MonoBehaviour
{
    public GameObject[] objectsToPlace; // מערך של אובייקטים להצבה
    public int numberOfObjectsToPlace = 10; // מספר האובייקטים להצבה
    public float maxDistanceFromPlayer = 5f; // מרחק מקסימלי מהשחקן להצבת האובייקטים
    public float minDistanceFromPlayer = 2f; // מרחק מינימלי מהשחקן להצבת האובייקטים
    public float minDistanceBetweenObjects = 1f; // מרחק מינימלי בין האובייקטים
    public float maxDistanceFromPlayerToDisappear = 10f; // מרחק מקסימלי מהשחקן לפני שהאובייקט מוחלף

    // תו לאזור האסור שבו אין להציב אובייקטים
    public string forbiddenTag = "shore";

    private List<GameObject> placedObjects = new List<GameObject>(); // רשימה לעקוב אחרי האובייקטים שהושמו
    private List<GameObject> availableObjects = new List<GameObject>(); // רשימה לאובייקטים זמינים להצבה

    void Start()
    {
        // אתחול הרשימה של האובייקטים הזמינים
        availableObjects = new List<GameObject>(objectsToPlace);
        PlaceObjectsRandomly();
    }

    void Update()
    {
        // בדיקה האם אובייקטים רחוקים מדי מהשחקן או בתוך אזור האסור, והחלפתם
        CheckAndReplaceObjects();
    }

    void PlaceObjectsRandomly()
    {
        if (objectsToPlace.Length == 0) return;

        // הצבת מספר האובייקטים הנדרש
        while (placedObjects.Count < numberOfObjectsToPlace)
        {
            // קבלת אובייקט ייחודי להצבה
            GameObject objectToPlace = GetUniqueObject();

            // יצירת מיקום אקראי סביב השחקן בטווח המוגדר
            Vector3 randomPosition = GetRandomPositionAroundPlayer();

            // בדיקה האם המיקום תקין (לא קרוב מדי לאובייקטים אחרים וגם לא באזור אסור)
            if (IsPositionValid(randomPosition) && !IsPositionInForbiddenArea(randomPosition))
            {
                // יצירת האובייקט במיקום האקראי
                GameObject placedObject = Instantiate(objectToPlace, randomPosition, Quaternion.identity);

                // מתן סיבוב אקראי לאובייקט
                placedObject.transform.Rotate(0, Random.Range(0f, 360f), 0);

                // הוספת האובייקט לרשימת האובייקטים שהושמו
                placedObjects.Add(placedObject);
            }
        }
    }

    // פונקציה לקבלת אובייקט ייחודי (מסירה מרשימת הזמינים)
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

    // פונקציה לקבלת מיקום אקראי בטווח (מינימום ומקסימום) מהשחקן
    Vector3 GetRandomPositionAroundPlayer()
    {
        float distance = Random.Range(minDistanceFromPlayer, maxDistanceFromPlayer);
        float angle = Random.Range(0f, Mathf.PI * 2);
        float x = Mathf.Cos(angle) * distance;
        float z = Mathf.Sin(angle) * distance;
        return new Vector3(x, 0, z) + transform.position;
    }

    // פונקציה לבדיקת תקינות המיקום (שהוא לא קרוב מדי לאובייקטים אחרים)
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

    // פונקציה שבודקת אם המיקום נמצא בתוך אחד מהאזורים עם התו האסור ("dint")
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

    // פונקציה שבודקת אם האובייקטים רחוקים מדי מהשחקן או נמצאים באזור האסור, ומחליפה אותם
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

                // אם המיקום החדש גם לא תקין או נמצא באזור אסור, ננסה מספר פעמים
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
