using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.InputSystem;

public class CustomNavigation : MonoBehaviour
{
    public ARPlaneManager arPlaneManager;
    public Camera arCamera;
    private GameObject selectedCat;


    private void Start()
    {
        if (arCamera == null)
        {
            Debug.LogError("AR Camera is not assigned in the Inspector.");
        }
        else
        {
            Debug.Log("AR Camera successfully assigned: " + arCamera.name);
        }
    }

    void Update()
    {
        // Check for Mouse input
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            Debug.Log("Mouse click detected.");
            HandleSelection();
            return;
        }

        // Check for Touch input
        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasPressedThisFrame)
        {
            Debug.Log("Touch input detected.");
            HandleSelection();
            return;
        }

        // If no input is detected
        if (Mouse.current == null && Touchscreen.current == null)
        {
            Debug.LogWarning("No valid input device detected! Ensure you're testing with a supported device.");
        }
    }


    private void HandleSelection()
    {
        // Perform a raycast from the touch or click position
        Vector2 touchPosition = GetTouchPosition();
        Ray ray = arCamera.ScreenPointToRay(touchPosition);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (hit.collider.CompareTag("Cat"))
            {
                // Select the cat
                selectedCat = hit.collider.gameObject;
                Debug.Log($"Selected cat: {selectedCat.name}");
            }
            else if (hit.collider.CompareTag("ARPlane") && selectedCat != null)
            {
                // Move the selected cat to the hit position
                Vector3 targetPosition = hit.point;
                selectedCat.GetComponent<CatMover>().MoveTo(targetPosition);
                Debug.Log("ANDITO NAKO");
            }
        }
    }

    private Vector2 GetTouchPosition()
    {
        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.isPressed)
        {
            // Get touch position
            return Touchscreen.current.primaryTouch.position.ReadValue();
        }
        else if (Mouse.current != null)
        {
            // Get mouse position
            return Mouse.current.position.ReadValue();
        }

        return Vector2.zero;
    }
}
