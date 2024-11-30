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
    private Coroutine spawnCoroutine; // Reference to the spawning coroutine

    void OnEnable()
    {
        // Restart the spawning coroutine when the spawner is enabled
        spawnCoroutine = StartCoroutine(SpawnObjects());
    }

    void OnDisable()
    {
        // Stop the spawning coroutine when the spawner is disabled
        if (spawnCoroutine != null)
        {
            StopCoroutine(spawnCoroutine);
        }

        // Clear all spawned objects
        ClearSpawnedObjects();
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
        const int maxRetries = 10;
        for (int i = 0; i < maxRetries; i++)
        {
            float randomX = Random.Range(-spawnRangeX, spawnRangeX);
            Vector3 spawnPosition = new Vector3(randomX, cloudPosition.position.y, cloudPosition.position.z);

            if (IsPositionValid(spawnPosition))
            {
                GameObject obj = objectArray[Random.Range(0, objectArray.Length)];
                GameObject spawnedObject = Instantiate(obj, spawnPosition, Quaternion.identity);
                spawnedObject.AddComponent<FallingObject>().fallSpeed = fallingSpeed;
                spawnedObjects.Add(spawnedObject);
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
                return false;
            }
        }
        return true;
    }

    public void ClearSpawnedObjects()
    {
        foreach (GameObject obj in spawnedObjects)
        {
            if (obj != null)
            {
                Destroy(obj);
            }
        }
        spawnedObjects.Clear();
    }
}
