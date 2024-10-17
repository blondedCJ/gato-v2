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
        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.isPressed)
        {
            Vector2 touchPosition = Touchscreen.current.primaryTouch.position.ReadValue();

            if (!catSpawned && arRaycastManager.Raycast(touchPosition, hits, TrackableType.PlaneWithinPolygon))
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
            // Select the first cat the user owns, for example
            int selectedCatIndex = userOwnedCats[0];
            if (selectedCatIndex >= 0 && selectedCatIndex < catPrefabs.Length)
            {
                GameObject spawnedCat = Instantiate(catPrefabs[selectedCatIndex], position, Quaternion.identity);
                Vector3 directionToCamera = arCamera.transform.position - spawnedCat.transform.position;
                directionToCamera.y = 0;
                spawnedCat.transform.rotation = Quaternion.LookRotation(directionToCamera);
                catSpawned = true;
            }
        } else
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
