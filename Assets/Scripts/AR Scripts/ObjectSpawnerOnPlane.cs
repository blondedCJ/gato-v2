using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class ObjectSpawnerOnPlane : MonoBehaviour
{
    [SerializeField] private GameObject[] catPrefabs; // Array for different cat prefabs
    private ARRaycastManager arRaycastManager;
    private Camera arCamera;
    private List<int> userOwnedCats;
    private UserInventory userInventory; // Reference to the UserInventory script
    private static List<ARRaycastHit> hits = new List<ARRaycastHit>();
    private bool catSpawned = false;
    private int currentCatIndex = 0;
    private bool allCatsSpawned = false; // Flag to prevent further spawning
    WalkablePlaneManager walkablePlaneManager;

    void Awake()
    {
        arRaycastManager = GetComponent<ARRaycastManager>();
        arCamera = Camera.main;

        userInventory = FindObjectOfType<UserInventory>();
        if (userInventory != null)
        {
            userInventory.LoadUserOwnedCats();
            userOwnedCats = userInventory.userOwnedCats;
        }
    }

    private void Start()
    {
        if (catPrefabs != null && catPrefabs.Length > 0)
        {
            Debug.Log("Cat Prefabs: " + catPrefabs.Length + " prefabs available");

            // Log each prefab's index and name (or other identifying properties)
            for (int i = 0; i < catPrefabs.Length; i++)
            {
                if (catPrefabs[i] != null)
                {
                    Debug.Log("Index " + i + ": " + catPrefabs[i].name);
                }
                else
                {
                    Debug.LogError("Prefab at index " + i + " is not assigned.");
                }
            }
        }
        else
        {
            Debug.LogError("Cat prefabs not assigned or empty");
        }
        walkablePlaneManager = GetComponent<WalkablePlaneManager>();
    }

    void Update()
    {
#if UNITY_EDITOR
        HandleMouseInput();
#else
        HandleTouchInput();
#endif
    }

    private void HandleTouchInput()
    {
        if (!allCatsSpawned && Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasPressedThisFrame)
        {
            Vector2 touchPosition = Touchscreen.current.primaryTouch.position.ReadValue();

            if (arRaycastManager.Raycast(touchPosition, hits, TrackableType.PlaneWithinPolygon))
            {
                Pose hitPose = hits[0].pose;
                SpawnUserOwnedCatAtPosition(hitPose.position);
            }
        }
    }

    private void HandleMouseInput()
    {
        if (!allCatsSpawned && Mouse.current.leftButton.wasPressedThisFrame)
        {
            Ray ray = arCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
            RaycastHit hit;

            if (!catSpawned && Physics.Raycast(ray, out hit))
            {
                if (arRaycastManager.Raycast(Mouse.current.position.ReadValue(), hits, TrackableType.PlaneWithinPolygon))
                {
                    Pose hitPose = hits[0].pose;
                    SpawnUserOwnedCatAtPosition(hitPose.position);
                }
            }
        }
    }

    private void SpawnUserOwnedCatAtPosition(Vector3 position)
    {
        if (userOwnedCats.Count > 0 && currentCatIndex >= 0)
        {
            Debug.Log("User Owned Cats: " + string.Join(", ", userOwnedCats));

            int selectedCatIndex = userOwnedCats[currentCatIndex];
            Debug.Log("Spawning Cat with Index: " + selectedCatIndex);

            if (selectedCatIndex >= 0 && selectedCatIndex < catPrefabs.Length)
            {
                GameObject spawnedCat = Instantiate(catPrefabs[selectedCatIndex], position, Quaternion.identity);
                Vector3 directionToCamera = arCamera.transform.position - spawnedCat.transform.position;
                directionToCamera.y = 0;
                spawnedCat.transform.rotation = Quaternion.LookRotation(directionToCamera);
                walkablePlaneManager.AddCatToList(spawnedCat);

                currentCatIndex++;
                if (currentCatIndex >= userOwnedCats.Count)
                {
                    Debug.Log("All cats spawned. No more cats to spawn.");
                    allCatsSpawned = true; // Set the flag to true, no more cats to spawn
                }
                else
                {
                    Debug.Log("Next Cat Index: " + currentCatIndex);
                }
            }
            else
            {
                Debug.LogError("Invalid selectedCatIndex: " + selectedCatIndex);
            }
        }
        else
        {
            Debug.Log("User has no cats or all cats already spawned.");
        }
    }

    private void LoadUserOwnedCats()
    {
        userOwnedCats = new List<int>();

        string ownedCatsString = PlayerPrefs.GetString("userOwnedCats", "");
        if (!string.IsNullOrEmpty(ownedCatsString))
        {
            string[] ownedCatsArray = ownedCatsString.Split(',');
            foreach (string catIndex in ownedCatsArray)
            {
                if (int.TryParse(catIndex, out int index))
                {
                    userOwnedCats.Add(index);
                }
            }
        }
    }
}
