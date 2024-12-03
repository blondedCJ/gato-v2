using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;

public class FurnitureManager : MonoBehaviour
{
    public enum Furniture
    {
        CatTower, DonutBed, CatSofa, CatScratcher
    }

    public GameObject FurnitureTab;
    public ARRaycastManager arRaycastManager;

    [Serializable]
    public class FurnitureButtonMapping
    {
        public Furniture furniture;
        public Button button;
        public GameObject prefab;
        public float yOffset; // Add Y-offset for each furniture
    }

    public List<FurnitureButtonMapping> furnitureButtonMappings;
    private Dictionary<Furniture, Button> furnitureButtons = new Dictionary<Furniture, Button>();
    private Dictionary<Furniture, GameObject> furniturePrefabs = new Dictionary<Furniture, GameObject>();
    private Dictionary<Furniture, float> furnitureYOffsets = new Dictionary<Furniture, float>(); // Store Y-offsets
    private Dictionary<Furniture, GameObject> spawnedFurniture = new Dictionary<Furniture, GameObject>();

    private Furniture? selectedFurniture = null; // Ensure this is declared
    private GameObject selectedFurnitureObject = null;

    private void Start()
    {
        foreach (var mapping in furnitureButtonMappings)
        {
            furnitureButtons[mapping.furniture] = mapping.button;
            furniturePrefabs[mapping.furniture] = mapping.prefab;
            furnitureYOffsets[mapping.furniture] = mapping.yOffset; // Initialize Y-offsets

            // Add listener for button clicks
            mapping.button.onClick.AddListener(() => SelectFurnitureType(mapping.furniture));
        }
    }

    private void Update()
    {
        if (FurnitureTab.activeSelf)
        {
#if UNITY_EDITOR
            HandleMouseInput();
#else
            HandleTouchInput();
#endif
        }
    }

    private void HandleMouseInput()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Vector2 screenPosition = Mouse.current.position.ReadValue();
            HandleFurniturePlacementOrSelection(screenPosition);
        }

        if (Mouse.current.leftButton.isPressed && selectedFurnitureObject != null)
        {
            Vector2 screenPosition = Mouse.current.position.ReadValue();
            DragSelectedFurniture(screenPosition);
        }
    }

    private void HandleTouchInput()
    {
        if (Touchscreen.current != null)
        {
            var touch = Touchscreen.current.primaryTouch;

            if (touch.press.wasPressedThisFrame)
            {
                Vector2 screenPosition = touch.position.ReadValue();
                HandleFurniturePlacementOrSelection(screenPosition);
            }

            if (touch.press.isPressed && selectedFurnitureObject != null)
            {
                Vector2 screenPosition = touch.position.ReadValue();
                DragSelectedFurniture(screenPosition);
            }
        }
    }

    private void HandleFurniturePlacementOrSelection(Vector2 screenPosition)
    {
        List<ARRaycastHit> hits = new List<ARRaycastHit>();
        if (arRaycastManager.Raycast(screenPosition, hits, UnityEngine.XR.ARSubsystems.TrackableType.PlaneWithinBounds))
        {
            Pose hitPose = hits[0].pose;

            if (selectedFurnitureObject == null)
            {
                // Check if a furniture object was clicked
                Ray ray = Camera.main.ScreenPointToRay(screenPosition);
                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    GameObject clickedObject = hit.collider.gameObject;
                    if (spawnedFurniture.ContainsValue(clickedObject))
                    {
                        selectedFurnitureObject = clickedObject;
                        Debug.Log($"Selected {clickedObject.name} for repositioning.");
                        return;
                    }
                }

                // Place new furniture with Y-offset
                if (selectedFurniture.HasValue && !spawnedFurniture.ContainsKey((Furniture)selectedFurniture))
                {
                    GameObject prefab = furniturePrefabs[(Furniture)selectedFurniture];
                    float yOffset = furnitureYOffsets[(Furniture)selectedFurniture]; // Get Y-offset
                    Vector3 adjustedPosition = new Vector3(hitPose.position.x, hitPose.position.y + yOffset, hitPose.position.z);

                    GameObject newFurniture = Instantiate(prefab, adjustedPosition, hitPose.rotation);
                    spawnedFurniture[(Furniture)selectedFurniture] = newFurniture;
                    Debug.Log($"{selectedFurniture} placed at adjusted height.");
                }
            }
            else
            {
                // Deselect the furniture
                selectedFurnitureObject = null;
                Debug.Log("Deselected furniture.");
            }
        }
        else
        {
            Debug.LogWarning("No AR plane detected at the clicked position.");
        }
    }


    private void DragSelectedFurniture(Vector2 screenPosition)
    {
        List<ARRaycastHit> hits = new List<ARRaycastHit>();
        if (arRaycastManager.Raycast(screenPosition, hits, UnityEngine.XR.ARSubsystems.TrackableType.PlaneWithinBounds))
        {
            Pose hitPose = hits[0].pose;

            // Get the Y-offset of the currently dragged furniture
            Furniture? furnitureType = GetFurnitureTypeByGameObject(selectedFurnitureObject);
            if (furnitureType.HasValue)
            {
                float yOffset = furnitureYOffsets[furnitureType.Value];
                Vector3 adjustedPosition = new Vector3(hitPose.position.x, hitPose.position.y + yOffset, hitPose.position.z);

                selectedFurnitureObject.transform.position = adjustedPosition;
                Debug.Log($"Repositioned {selectedFurnitureObject.name} with offset.");
            }
            else
            {
                Debug.LogWarning("Failed to identify furniture type for repositioning.");
            }
        }
        else
        {
            Debug.LogWarning("Failed to detect AR plane while dragging.");
        }
    }



    public void SelectFurnitureType(Furniture furniture)
    {
        if (spawnedFurniture.ContainsKey(furniture))
        {
            Debug.LogWarning($"{furniture} has already been placed.");
            return;
        }

        selectedFurniture = furniture;
        Debug.Log($"{furniture} selected for placement.");
    }

    public void ClearSelection()
    {
        selectedFurnitureObject = null;
        Debug.Log("Cleared furniture selection.");
    }

    private Furniture? GetFurnitureTypeByGameObject(GameObject furnitureObject)
    {
        foreach (var kvp in spawnedFurniture)
        {
            if (kvp.Value == furnitureObject)
            {
                return kvp.Key;
            }
        }
        return null;
    }

}
