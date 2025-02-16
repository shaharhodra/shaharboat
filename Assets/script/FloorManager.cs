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
    [Header("הגדרות השחקן והרצפות")]
    public Transform player;                // הפניה לשחקן
    public List<FloorPrefab> floorPrefabs;  // רשימה של פרפבים עם משקלים
    public float tileSize = 10f;            // גודל כל רצפה

    // תא מרכזי במערך (Grid) שמייצג את הרצפה תחת השחקן
    private Vector2Int currentCenter;
    // מילון שמחזיק את כל הרצפות הפעילות במערך עם מיקומן (בציר X וה-Z)
    private Dictionary<Vector2Int, GameObject> activeTiles = new Dictionary<Vector2Int, GameObject>();

    void Start()
    {
        // חישוב תא מרכזי ראשוני בהתבסס על מיקום השחקן
        currentCenter = new Vector2Int(
            Mathf.RoundToInt(player.position.x / tileSize),
            Mathf.RoundToInt(player.position.z / tileSize)
        );
        UpdateTiles();
    }

    void Update()
    {
        // חשב תא חדש לפי מיקום השחקן הנוכחי
        Vector2Int newCenter = new Vector2Int(
            Mathf.RoundToInt(player.position.x / tileSize),
            Mathf.RoundToInt(player.position.z / tileSize)
        );

        // אם השחקן עבר לתא חדש, עדכן את הרצפות
        if (newCenter != currentCenter)
        {
            currentCenter = newCenter;
            UpdateTiles();
        }
    }

    /// <summary>
    /// פונקציה זו בוחרת אקראית פרפב מתוך הרשימה בהתבסס על המשקלים שהוגדרו
    /// </summary>
    /// <returns>אובייקט פרפב נבחר</returns>
    GameObject GetRandomFloorPrefab()
    {
        // חישוב סך כל המשקלים
        float totalWeight = 0f;
        foreach (FloorPrefab fp in floorPrefabs)
        {
            totalWeight += fp.weight;
        }

        // בוחרים מספר רנדומלי בין 0 ל־totalWeight
        float randomValue = Random.Range(0, totalWeight);

        // מעבר על הרשימה עד שהסכום עובר את הערך הרנדומלי
        foreach (FloorPrefab fp in floorPrefabs)
        {
            randomValue -= fp.weight;
            if (randomValue <= 0)
            {
                return fp.prefab;
            }
        }

        // במקרה חריג, מחזירים את הפרפב הראשון
        return floorPrefabs[0].prefab;
    }

    /// <summary>
    /// דואגת לכך שבסביבת תא השחקן יהיו תמיד 9 רצפות (מערך 3×3)
    /// </summary>
    void UpdateTiles()
    {
        // מילון זמני לאחסון הרצפות שיש להישאר
        Dictionary<Vector2Int, GameObject> newTiles = new Dictionary<Vector2Int, GameObject>();

        // עבור כל תא במערך 3x3 סביב המרכז (כולל מרכז, ארבעת הצדדים והאלכסונים)
        for (int offsetX = -1; offsetX <= 1; offsetX++)
        {
            for (int offsetZ = -1; offsetZ <= 1; offsetZ++)
            {
                Vector2Int tileCoord = new Vector2Int(currentCenter.x + offsetX, currentCenter.y + offsetZ);

                // אם הרצפה קיימת כבר במיקום הזה, מעבירים אותה למילון החדש
                if (activeTiles.ContainsKey(tileCoord))
                {
                    newTiles[tileCoord] = activeTiles[tileCoord];
                    activeTiles.Remove(tileCoord);
                }
                else
                {
                    // אחרת, נוצר אובייקט רצפה חדש
                    Vector3 spawnPos = new Vector3(tileCoord.x * tileSize, 0, tileCoord.y * tileSize);
                    // בחירת פרפב בהתבסס על המשקלים שהוגדרו
                    GameObject tilePrefab = GetRandomFloorPrefab();
                    GameObject newTile = Instantiate(tilePrefab, spawnPos, Quaternion.identity);
                    newTiles[tileCoord] = newTile;
                }
            }
        }

        // השמדת הרצפות שלא נמצאות במערך 3x3 החדש
        foreach (KeyValuePair<Vector2Int, GameObject> tile in activeTiles)
        {
            Destroy(tile.Value);
        }

        // עדכון הרשימה של הרצפות הפעילות
        activeTiles = newTiles;
    }
}
