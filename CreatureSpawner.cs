using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatureSpawner : MonoBehaviour
{
    public GameObject objectToDuplicate; // The object you want to duplicate.
    public int numberOfDuplicates = 100; // Number of duplicates to create.
    public Vector2 mapSize = new Vector2(60f, 40f); // The size of the designated map area.

    void awake()
    {
        
    }
    public void CreatureSpawn()
    {        
        GameObject[] creatures = GameObject.FindGameObjectsWithTag("Creature");

        // Calculate the length once to avoid multiple calls
        int creaturesLength = creatures.Length;
        float floatValue = creaturesLength + Random.Range(-creaturesLength / 6, (int)(creaturesLength * 1.3f));

        // Ensure floatValue stays within valid bounds
        floatValue = Mathf.Clamp(floatValue, 0, Mathf.Infinity);

        numberOfDuplicates = Mathf.RoundToInt(floatValue);
        // Create duplicates
        for (int i = 0; i < numberOfDuplicates; i++)
        {
            // Generate random position within the map bounds
            Vector3 randomPosition = new Vector3(
                Random.Range(-mapSize.x / 2, mapSize.x / 2),
                Random.Range(-mapSize.y / 2, mapSize.y / 2), 0f
            );

            // Create a new instance of the objectToDuplicate at the random position
            Instantiate(objectToDuplicate, randomPosition, Quaternion.identity);
        }
    }
    void Start()
    {
        GameObject[] creatures = GameObject.FindGameObjectsWithTag("Creature");

        // Calculate the length once to avoid multiple calls
        int creaturesLength = creatures.Length;
        float floatValue = creaturesLength + Random.Range(-creaturesLength / 6, (int)(creaturesLength * 1.3f));

        // Ensure floatValue stays within valid bounds
        floatValue = Mathf.Clamp(floatValue, 0, Mathf.Infinity);

        numberOfDuplicates = Mathf.RoundToInt(floatValue);
        // Create duplicates
        for (int i = 0; i < numberOfDuplicates; i++)
        {
            // Generate random position within the map bounds
            Vector3 randomPosition = new Vector3(
                Random.Range(-mapSize.x / 2, mapSize.x / 2),
                Random.Range(-mapSize.y / 2, mapSize.y / 2), 0f
            );

            // Create a new instance of the objectToDuplicate at the random position
            Instantiate(objectToDuplicate, randomPosition, Quaternion.identity);
        }
    }
}
