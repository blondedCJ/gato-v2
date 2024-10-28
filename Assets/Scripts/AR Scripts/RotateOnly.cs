using UnityEngine;
using UnityEngine.InputSystem;

public class RotateOnly : MonoBehaviour
{
    public Vector3 rotationSpeed = new Vector3(0, 50, 0); // Rotation speed in degrees per second for each axis
    public Vector3 scaleIncrease = new Vector3(0.3f, 0.3f, 0.3f); // Scale increase when held down
    public float scaleSpeed = 5f; // Speed of scaling up/down for this object
    public GameObject catParent; // Reference to the CatParent GameObject in the Inspector
    public float catScaleSpeed = 2f; // Speed of scaling for catParent object
    public Vector3 catScaleIncrease = new Vector3(0.2f, 0.2f, 0.2f); // Initial scale increase for catParent

    private Vector3 targetScale; // Target scale to interpolate towards
    private Vector3 originalScale;
    private bool wasHeld = false; // Track if the object was held down

    void Start()
    {
        originalScale = transform.localScale; // Store the original scale of the object
        targetScale = originalScale; // Initialize target scale
    }

    void Update()
    {
        Rotate();

        // Check if the object is being held
        if (IsPressed())
        {
            targetScale = originalScale + scaleIncrease;
            wasHeld = true;
        }
        else
        {
            targetScale = originalScale;

            // If the user releases after holding, disable this object and enable the CatParent object
            if (wasHeld)
            {
                gameObject.SetActive(false);
                EnableCatParent();
                wasHeld = false; // Reset the flag
            }
        }

        // Smoothly interpolate towards the target scale
        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime * scaleSpeed);
    }

    private void Rotate()
    {
        // Rotate the object based on rotationSpeed and time
        transform.Rotate(rotationSpeed * Time.deltaTime);
    }

    private bool IsPressed()
    {
        // For editor: check if left mouse button is held down
        if (Mouse.current != null && Mouse.current.leftButton.isPressed)
            return true;

        // For mobile: check if there's an active primary touch
        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.isPressed)
            return true;

        return false;
    }

    private void EnableCatParent()
    {
        // Enable the CatParent GameObject and smoothly scale it up and back to its original size
        if (catParent != null)
        {
            catParent.SetActive(true);
            StartCoroutine(ScaleCatParentSmoothly());
        }
        else
        {
            Debug.LogWarning("CatParent GameObject is not assigned.");
        }
    }

    private System.Collections.IEnumerator ScaleCatParentSmoothly()
    {
        Vector3 catOriginalScale = catParent.transform.localScale;
        Vector3 catTargetScale = catOriginalScale + catScaleIncrease;

        // Scale up
        while (catParent.transform.localScale != catTargetScale)
        {
            catParent.transform.localScale = Vector3.Lerp(catParent.transform.localScale, catTargetScale, Time.deltaTime * catScaleSpeed);
            yield return null;
        }

        // Scale back to original size
        while (catParent.transform.localScale != catOriginalScale)
        {
            catParent.transform.localScale = Vector3.Lerp(catParent.transform.localScale, catOriginalScale, Time.deltaTime * catScaleSpeed);
            yield return null;
        }
    }
}
