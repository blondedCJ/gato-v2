using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class ObjectSpawnerOnPlane : MonoBehaviour
{
    [SerializeField]
    private GameObject cat; // Reference to the cat prefab
    private ARRaycastManager arRaycastManager; // Raycast manager to detect planes
    private Camera arCamera;
    private static List<ARRaycastHit> hits = new List<ARRaycastHit>();

    // Boolean to track if the cat has been spawned
    private bool catSpawned = false;

    void Awake()
    {
        // Get ARRaycastManager component
        arRaycastManager = GetComponent<ARRaycastManager>();
        arCamera = Camera.main;  // Get AR Camera
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
                SpawnCatAtPosition(hitPose.position);
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
                    SpawnCatAtPosition(hitPose.position);
                }
            }
        }
    }

    // Method to spawn cat object
    private void SpawnCatAtPosition(Vector3 position)
    {
        // Check if the cat has already been spawned


        // Instantiate the cat object at the specified position
        GameObject spawnedCat = Instantiate(cat, position, Quaternion.identity);

        // Make the cat object face the camera
        Vector3 directionToCamera = arCamera.transform.position - spawnedCat.transform.position;
        directionToCamera.y = 0; // Keep the rotation only on the y-axis
        spawnedCat.transform.rotation = Quaternion.LookRotation(directionToCamera); // Face toward the camera

        // Set the flag to true, indicating the cat has been spawned
        catSpawned = true;
    }
}
