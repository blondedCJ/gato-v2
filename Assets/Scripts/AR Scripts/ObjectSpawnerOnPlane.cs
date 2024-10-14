using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class ObjectSpawnerOnPlane : MonoBehaviour
{
    [SerializeField]
    private GameObject objectToSpawn;  // The prefab to spawn
    private ARRaycastManager arRaycastManager; // Raycast manager to detect planes
    private Camera arCamera;
    private static List<ARRaycastHit> hits = new List<ARRaycastHit>();

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

            if (arRaycastManager.Raycast(touchPosition, hits, TrackableType.PlaneWithinPolygon))
            {
                Pose hitPose = hits[0].pose;
                SpawnObjectAtPosition(hitPose.position);
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

            if (Physics.Raycast(ray, out hit))
            {
                // Cast against AR planes to simulate placement
                if (arRaycastManager.Raycast(Mouse.current.position.ReadValue(), hits, TrackableType.PlaneWithinPolygon))
                {
                    Pose hitPose = hits[0].pose;
                    SpawnObjectAtPosition(hitPose.position);
                }
            }
        }
    }


    // Method to spawn object and set isIdling to true, with the object facing the camera
    // Method to spawn object and set isIdling to true, with the object facing the camera
    private void SpawnObjectAtPosition(Vector3 position)
    {
        // Instantiate the object at the specified position
        GameObject spawnedObject = Instantiate(objectToSpawn, position, Quaternion.identity);

        // Make the object face the camera
        Vector3 directionToCamera = arCamera.transform.position - spawnedObject.transform.position;
        directionToCamera.y = 0; // Keep the rotation only on the y-axis, so the object doesn't tilt up/down
        spawnedObject.transform.rotation = Quaternion.LookRotation(-directionToCamera); // Face toward the camera

        // Rotate the object 180 degrees on the Y-axis to make it face the camera properly
        spawnedObject.transform.Rotate(0, 180, 0);

        // Get the Animator component from the spawned object
        Animator objectAnimator = spawnedObject.GetComponent<Animator>();

        // Check if the Animator component exists and set the isIdling parameter to true
    }


}
