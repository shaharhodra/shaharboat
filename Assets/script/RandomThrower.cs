using UnityEngine;

public class RandomThrower : MonoBehaviour
{
    public GameObject[] objectsToThrow; // Array to hold different objects to throw
    public float throwHeight = 5f; // Height at which objects will be placed
    public float distanceFromPlayer = 2f; // Distance from player where objects will be placed
    public float distanceBetweenObjects = 1f; // Distance between the objects

    private float timeSinceLastThrow;

    void Start()
    {
        timeSinceLastThrow = distanceBetweenObjects; // Initialize to the distance to prevent immediate instantiation
    }

    void Update()
    {
        timeSinceLastThrow += Time.deltaTime;

        // Instead of an interval, just place objects when desired
        if (timeSinceLastThrow >= distanceBetweenObjects)
        {
            PlaceObjectInFront();
            timeSinceLastThrow = 0f;
        }
    }

    void PlaceObjectInFront()
    {
        if (objectsToThrow.Length == 0) return;

        // Randomly choose an object from the array
        GameObject objectToPlace = objectsToThrow[Random.Range(0, objectsToThrow.Length)];

        // Calculate the position where the object will be placed in front of the player
        Vector3 placePosition = transform.position + transform.forward * distanceFromPlayer + Vector3.up * throwHeight;

        // Instantiate the object at the calculated position
        GameObject placedObject = Instantiate(objectToPlace, placePosition, Quaternion.identity);

        // Set a random rotation on the object
        placedObject.transform.Rotate(0, Random.Range(0f, 360f), 0);

        // Optionally, if you want to place additional objects next to it:
        PlaceAdditionalObjects(placePosition);
    }

    void PlaceAdditionalObjects(Vector3 firstObjectPosition)
    {
        for (float i = 1; i <= distanceBetweenObjects; i++)
        {
            // Place objects next to the first one on the right side
            Vector3 position = firstObjectPosition + transform.right * i;
            GameObject placedObject = Instantiate(objectsToThrow[Random.Range(0, objectsToThrow.Length)], position, Quaternion.identity);

            // Set a random rotation on the additional object
            placedObject.transform.Rotate(0, Random.Range(0f, 360f), 0);
        }
    }
}
