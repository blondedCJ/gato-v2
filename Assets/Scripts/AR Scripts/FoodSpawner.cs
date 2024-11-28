using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject[] foodObjects; // Array of food prefabs
    public GameObject[] dangerousObjects; // Array of dangerous prefabs
    public Transform cloudPosition; // Reference to the cloud position
    public float spawnInterval = 2f; // Time between spawns
    public float spawnRangeX = 5f; // Range on the X-axis for spawning
    public float fallingSpeed = 3f; // Falling speed of objects
    public float minSpawnDistance = 1.5f; // Minimum distance between spawned objects

    private List<GameObject> spawnedObjects = new List<GameObject>(); // Track spawned objects

    void Start()
    {
        StartCoroutine(SpawnObjects());
    }

    private IEnumerator SpawnObjects()
    {
        while (true)
        {
            spawnedObjects.Add(SpawnUniqueRandomObject(foodObjects));
            spawnedObjects.Add(SpawnUniqueRandomObject(dangerousObjects));
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private GameObject SpawnUniqueRandomObject(GameObject[] objectArray)
    {
        // Try to spawn an object within a max retry limit
        const int maxRetries = 10;
        for (int i = 0; i < maxRetries; i++)
        {
            // Determine random spawn position
            float randomX = Random.Range(-spawnRangeX, spawnRangeX);
            Vector3 spawnPosition = new Vector3(randomX, cloudPosition.position.y, cloudPosition.position.z);

            // Check for overlap
            if (IsPositionValid(spawnPosition))
            {
                // Spawn the object
                GameObject obj = objectArray[Random.Range(0, objectArray.Length)];
                GameObject spawnedObject = Instantiate(obj, spawnPosition, Quaternion.identity);

                // Add falling behavior
                spawnedObject.AddComponent<FallingObject>().fallSpeed = fallingSpeed;

                // Track the spawned object
                spawnedObjects.Add(spawnedObject);

                // Return the spawned object
                return spawnedObject;
            }
        }

        Debug.LogWarning("Could not find a valid spawn position after retries.");
        return null;
    }

    private bool IsPositionValid(Vector3 position)
    {
        foreach (GameObject obj in spawnedObjects)
        {
            if (obj != null && Vector3.Distance(position, obj.transform.position) < minSpawnDistance)
            {
                return false; // Overlap detected
            }
        }
        return true;
    }

    public void ClearSpawnedObjects()
    {
        // Destroy all active spawned objects
        foreach (GameObject obj in spawnedObjects)
        {
            if (obj != null)
            {
                Destroy(obj);
            }
        }
        Time.timeScale = 1f;
        // Clear the list
        spawnedObjects.Clear();
    }
}
