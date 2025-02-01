using UnityEngine;
using System.Collections.Generic;

public class RandomObjectPlacer : MonoBehaviour
{
    public GameObject[] objectsToPlace; // Array to hold different objects to place
    public int numberOfObjectsToPlace = 10; // Number of objects to place
    public float maxDistanceFromPlayer = 5f; // Maximum distance from player where objects will be placed
    public float minDistanceBetweenObjects = 1f; // Minimum distance between the objects
    public float maxDistanceFromPlayerToDisappear = 10f; // Max distance before objects disappear

    private List<GameObject> placedObjects = new List<GameObject>(); // List to track placed objects
    private List<GameObject> availableObjects = new List<GameObject>(); // List to hold available objects for placement

    void Start()
    {
        // Initialize the list of available objects
        availableObjects = new List<GameObject>(objectsToPlace);

        PlaceObjectsRandomly();
    }

    void Update()
    {
        // Check if any placed object is too far from the player and replace it
        CheckAndReplaceObjects();
    }

    void PlaceObjectsRandomly()
    {
        if (objectsToPlace.Length == 0) return;

        // Place the specified number of objects
        while (placedObjects.Count < numberOfObjectsToPlace)
        {
            // Get a unique object to place
            GameObject objectToPlace = GetUniqueObject();

            // Generate a random position around the player within maxDistanceFromPlayer
            Vector3 randomPosition = GetRandomPositionAroundPlayer();

            // Check if the position is valid (i.e. not too close to other objects)
            if (IsPositionValid(randomPosition))
            {
                // Instantiate the object at the random position
                GameObject placedObject = Instantiate(objectToPlace, randomPosition, Quaternion.identity);

                // Set a random rotation on the object
                placedObject.transform.Rotate(0, Random.Range(0f, 360f), 0);

                // Add the placed object to the list of placed objects
                placedObjects.Add(placedObject);
            }
        }
    }

    // Function to get a unique object (remove from available objects)
    GameObject GetUniqueObject()
    {
        // If we have already used all objects, we add them back for reuse
        if (availableObjects.Count == 0)
        {
            availableObjects = new List<GameObject>(objectsToPlace);
        }

        // Randomly select an object from the list of available objects
        GameObject uniqueObject = availableObjects[Random.Range(0, availableObjects.Count)];

        // Remove the selected object from the available objects list
        availableObjects.Remove(uniqueObject);

        return uniqueObject;
    }

    // Function to get a random position within the max distance from the player
    Vector3 GetRandomPositionAroundPlayer()
    {
        // Get random position in a circle around the player within maxDistanceFromPlayer
        Vector2 randomCircle = Random.insideUnitCircle * maxDistanceFromPlayer;
        Vector3 randomPosition = new Vector3(randomCircle.x, 0, randomCircle.y) + transform.position;

        return randomPosition;
    }

    // Function to check if the position is valid (not too close to other objects)
    bool IsPositionValid(Vector3 position)
    {
        foreach (GameObject placedObject in placedObjects)
        {
            if (Vector3.Distance(position, placedObject.transform.position) < minDistanceBetweenObjects)
            {
                return false; // If the position is too close to another object, it's invalid
            }
        }
        return true;
    }

    // Function to check if objects are too far from the player and replace them
    void CheckAndReplaceObjects()
    {
        for (int i = placedObjects.Count - 1; i >= 0; i--)
        {
            GameObject placedObject = placedObjects[i];

            // Check the distance between the placed object and the player (assuming the player is at the script's position)
            float distanceFromPlayer = Vector3.Distance(placedObject.transform.position, transform.position);

            // If the object is farther than the max allowed distance, replace it
            if (distanceFromPlayer > maxDistanceFromPlayerToDisappear)
            {
                // Destroy the object
                Destroy(placedObject);

                // Get a new object to place and generate a new position
                GameObject objectToPlace = GetUniqueObject();
                Vector3 newRandomPosition = GetRandomPositionAroundPlayer();

                // Instantiate the new object at the new position
                GameObject newPlacedObject = Instantiate(objectToPlace, newRandomPosition, Quaternion.identity);

                // Set a random rotation on the new object
                newPlacedObject.transform.Rotate(0, Random.Range(0f, 360f), 0);

                // Add the new placed object to the list of placed objects
                placedObjects[i] = newPlacedObject; // Replace the old object with the new one
            }
        }
    }
}
