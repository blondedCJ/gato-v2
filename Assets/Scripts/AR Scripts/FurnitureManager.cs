using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
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
    private Dictionary<Furniture, float> furnitureYOffsets = new Dictionary<Furniture, float>();
    private Dictionary<Furniture, GameObject> spawnedFurniture = new Dictionary<Furniture, GameObject>();

    private Furniture? selectedFurniture = null;
    private GameObject selectedFurnitureObject = null;

    // For tracking double-click
    private float lastClickTime = 0f;
    private const float doubleClickThreshold = 0.3f;

    private void Start() {
        foreach (var mapping in furnitureButtonMappings) {
            furnitureButtons[mapping.furniture] = mapping.button;
            furniturePrefabs[mapping.furniture] = mapping.prefab;
            furnitureYOffsets[mapping.furniture] = mapping.yOffset;

            // Add listener for button clicks
            mapping.button.onClick.AddListener(() => SelectFurnitureType(mapping.furniture));
        }
    }

    private void Update() {
        if (FurnitureTab.activeSelf) {
#if UNITY_EDITOR
            HandleMouseInput();
#else
            HandleTouchInput();
#endif
        }
    }

    private void HandleMouseInput() {
        if (EventSystem.current.IsPointerOverGameObject()) {
            return;
        }

        if (Mouse.current.leftButton.wasPressedThisFrame) {
            Vector2 screenPosition = Mouse.current.position.ReadValue();
            float timeSinceLastClick = Time.time - lastClickTime;

            if (timeSinceLastClick < doubleClickThreshold) {
                HandleFurnitureRemoval(screenPosition);
            } else {
                HandleFurniturePlacementOrSelection(screenPosition);
            }

            lastClickTime = Time.time;
        }

        if (Mouse.current.leftButton.isPressed && selectedFurnitureObject != null) {
            Vector2 screenPosition = Mouse.current.position.ReadValue();
            DragSelectedFurniture(screenPosition);
        }
    }

    private void HandleTouchInput() {
        if (Touchscreen.current != null) {
            var touch = Touchscreen.current.primaryTouch;

            if (touch.press.wasPressedThisFrame) {
                if (EventSystem.current.IsPointerOverGameObject((int)touch.touchId.ReadValue())) {
                    return;
                }

                Vector2 screenPosition = touch.position.ReadValue();
                float timeSinceLastClick = Time.time - lastClickTime;

                if (timeSinceLastClick < doubleClickThreshold) {
                    HandleFurnitureRemoval(screenPosition);
                } else {
                    HandleFurniturePlacementOrSelection(screenPosition);
                }

                lastClickTime = Time.time;
            }

            if (touch.press.isPressed && selectedFurnitureObject != null) {
                if (EventSystem.current.IsPointerOverGameObject((int)touch.touchId.ReadValue())) {
                    return;
                }

                Vector2 screenPosition = touch.position.ReadValue();
                DragSelectedFurniture(screenPosition);
            }
        }
    }

    private void HandleFurnitureRemoval(Vector2 screenPosition) {
        Ray ray = Camera.main.ScreenPointToRay(screenPosition);
        if (Physics.Raycast(ray, out RaycastHit hit)) {
            GameObject clickedObject = hit.collider.gameObject;
            if (spawnedFurniture.ContainsValue(clickedObject)) {
                Furniture? furnitureType = GetFurnitureTypeByGameObject(clickedObject);
                if (furnitureType.HasValue) {
                    spawnedFurniture.Remove(furnitureType.Value);
                    Destroy(clickedObject);
                    selectedFurnitureObject = null;
                    Debug.Log($"{furnitureType} removed on double-click.");
                }
            }
        }
    }

    private void HandleFurniturePlacementOrSelection(Vector2 screenPosition) {
        List<ARRaycastHit> hits = new List<ARRaycastHit>();
        if (arRaycastManager.Raycast(screenPosition, hits, UnityEngine.XR.ARSubsystems.TrackableType.PlaneWithinBounds)) {
            Pose hitPose = hits[0].pose;

            if (selectedFurnitureObject == null) {
                Ray ray = Camera.main.ScreenPointToRay(screenPosition);
                if (Physics.Raycast(ray, out RaycastHit hit)) {
                    GameObject clickedObject = hit.collider.gameObject;
                    if (spawnedFurniture.ContainsValue(clickedObject)) {
                        selectedFurnitureObject = clickedObject;
                        Debug.Log($"Selected {clickedObject.name} for repositioning.");
                        return;
                    }
                }

                if (selectedFurniture.HasValue && !spawnedFurniture.ContainsKey((Furniture)selectedFurniture)) {
                    GameObject prefab = furniturePrefabs[(Furniture)selectedFurniture];
                    float yOffset = furnitureYOffsets[(Furniture)selectedFurniture];
                    Vector3 adjustedPosition = new Vector3(hitPose.position.x, hitPose.position.y + yOffset, hitPose.position.z);

                    GameObject newFurniture = Instantiate(prefab, adjustedPosition, hitPose.rotation);
                    spawnedFurniture[(Furniture)selectedFurniture] = newFurniture;
                    Debug.Log($"{selectedFurniture} placed at adjusted height.");
                }
            } else {
                selectedFurnitureObject = null;
                Debug.Log("Deselected furniture.");
            }
        } else {
            Debug.LogWarning("No AR plane detected at the clicked position.");
        }
    }

    private void DragSelectedFurniture(Vector2 screenPosition) {
        List<ARRaycastHit> hits = new List<ARRaycastHit>();
        if (arRaycastManager.Raycast(screenPosition, hits, UnityEngine.XR.ARSubsystems.TrackableType.PlaneWithinBounds)) {
            Pose hitPose = hits[0].pose;

            Furniture? furnitureType = GetFurnitureTypeByGameObject(selectedFurnitureObject);
            if (furnitureType.HasValue) {
                float yOffset = furnitureYOffsets[furnitureType.Value];
                Vector3 adjustedPosition = new Vector3(hitPose.position.x, hitPose.position.y + yOffset, hitPose.position.z);

                selectedFurnitureObject.transform.position = adjustedPosition;
                Debug.Log($"Repositioned {selectedFurnitureObject.name} with offset.");
            } else {
                Debug.LogWarning("Failed to identify furniture type for repositioning.");
            }
        } else {
            Debug.LogWarning("Failed to detect AR plane while dragging.");
        }
    }
    public void SelectFurnitureType(Furniture furniture) {
        // If the currently selected furniture is clicked again, deselect it
        if (selectedFurniture == furniture) {
            ClearSelection();
            ResetButtonHighlights();
            Debug.Log($"{furniture} deselected.");
            return;
        }

        // Otherwise, select the new furniture and highlight the button
        selectedFurniture = furniture;
        Debug.Log($"{furniture} selected for placement.");
        HighlightSelectedButton(furniture);
    }


    public void ClearSelection() {
        selectedFurniture = null;
        selectedFurnitureObject = null;
        ResetButtonHighlights();
        Debug.Log("Cleared furniture selection.");
    }

    private Furniture? GetFurnitureTypeByGameObject(GameObject furnitureObject) {
        foreach (var kvp in spawnedFurniture) {
            if (kvp.Value == furnitureObject) {
                return kvp.Key;
            }
        }
        return null;
    }

    public void RemoveAllFurniture() {
        foreach (var kvp in spawnedFurniture) {
            if (kvp.Value != null) {
                Destroy(kvp.Value);
            }
        }
        spawnedFurniture.Clear();
        selectedFurnitureObject = null;
        selectedFurniture = null;
        Debug.Log("All spawned furniture has been removed.");
    }

    // Highlight color for selected button
    public Color selectedColor = Color.yellow;
    // Default button color
    private Color defaultColor = Color.white;

    private void HighlightSelectedButton(Furniture selected) {
        foreach (var kvp in furnitureButtons) {
            if (kvp.Key == selected) {
                kvp.Value.image.color = selectedColor;
            } else {
                kvp.Value.image.color = defaultColor;
            }
        }
    }
    // Function to reset all button highlights (for deselection)
    private void ResetButtonHighlights() {
        foreach (var kvp in furnitureButtons) {
            kvp.Value.image.color = defaultColor;
        }
    }

}
