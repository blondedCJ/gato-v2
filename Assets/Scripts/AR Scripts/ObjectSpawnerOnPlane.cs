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

    // Method to handle touch input (New Input System) for mobile devices
    private void HandleTouchInput()
    {
        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasPressedThisFrame)
        {
            Vector2 touchPosition = Touchscreen.current.primaryTouch.position.ReadValue();

            if (arRaycastManager.Raycast(touchPosition, hits, TrackableType.PlaneWithinPolygon))
            {
                Pose hitPose = hits[0].pose;
                SpawnUserOwnedCatAtPosition(hitPose.position);
            }
        }
    }

    // Method to handle mouse input for Unity Editor testing
    private void HandleMouseInput()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Ray ray = arCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
            RaycastHit hit;

            if (!catSpawned && Physics.Raycast(ray, out hit))
            {
                // Cast against AR planes to simulate placement
                if (arRaycastManager.Raycast(Mouse.current.position.ReadValue(), hits, TrackableType.PlaneWithinPolygon))
                {
                    Pose hitPose = hits[0].pose;
                    SpawnUserOwnedCatAtPosition(hitPose.position);
                }
            }
        }
    }

    // Method to spawn a cat object based on user ownership
    private void SpawnUserOwnedCatAtPosition(Vector3 position)
    {
        if (userOwnedCats.Count > 0)
        {
            Debug.Log("User Owned Cats: " + string.Join(", ", userOwnedCats)); // Log the owned cats

            // Select the next cat in the user's owned list
            int selectedCatIndex = userOwnedCats[currentCatIndex];
            Debug.Log("Spawning Cat with Index: " + selectedCatIndex); // Log the selected cat

            if (selectedCatIndex >= 0 && selectedCatIndex < catPrefabs.Length)
            {
                GameObject spawnedCat = Instantiate(catPrefabs[selectedCatIndex], position, Quaternion.identity);
                Vector3 directionToCamera = arCamera.transform.position - spawnedCat.transform.position;
                directionToCamera.y = 0;
                spawnedCat.transform.rotation = Quaternion.LookRotation(directionToCamera);
                walkablePlaneManager.AddCatToList(spawnedCat);

                

                // Increment the index and wrap around if needed
                currentCatIndex++;
                if (currentCatIndex >= userOwnedCats.Count)
                {
                    Debug.Log("All cats spawned. No more cats to spawn."); // Log when all cats are spawned
                    currentCatIndex = -1; // Set to -1 so the next call to increment will go back to 0
                }
                else
                {
                    Debug.Log("Next Cat Index: " + currentCatIndex); // Log the next index
                }
            }
            else
            {
                Debug.LogError("Invalid selectedCatIndex: " + selectedCatIndex);
            }
        }
        else
        {
            Debug.Log("User has no cats");
        }
    }




    // Load user-owned cats from PlayerPrefs
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
