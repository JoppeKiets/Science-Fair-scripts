using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatureSpawner : MonoBehaviour
{
    public GameObject creature; // The object you want to duplicate.
    public GameObject enemy; // The object you want to duplicate.
    public int numberOfDuplicates = 1; // Number of duplicates to create.
    public Vector2 mapSize = new Vector2(60f, 40f); // The size of the designated map area.

    void awake()
    {
        
    }
    void start()
    {
        // Create duplicates
        for (int i = 0; i < numberOfDuplicates; i++)
        {
            // Generate random position within the map bounds
            Vector3 randomPosition = new Vector3(
                Random.Range(-mapSize.x / 2, mapSize.x / 2),
                Random.Range(-mapSize.y / 2, mapSize.y / 2), 0f
            );

            // Create a new instance of the objectToDuplicate at the random position
            Instantiate(creature, randomPosition, Quaternion.identity);
        }
        // Create duplicates
        for (int i = 0; i < numberOfDuplicates; i++)
        {
            // Generate random position within the map bounds
            Vector3 randomPosition = new Vector3(
                Random.Range(-mapSize.x / 2, mapSize.x / 2),
                Random.Range(-mapSize.y / 2, mapSize.y / 2), 0f
            );

            // Create a new instance of the objectToDuplicate at the random position
            Instantiate(enemy, randomPosition, Quaternion.identity);
        }
    }

}