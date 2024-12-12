using UnityEngine;

public class RotateCat : MonoBehaviour
{
    public Vector3 rotationSpeed = new Vector3(0, 50, 0); // Rotation speed in degrees per second for each axis
    public Vector3 scaleIncrease = new Vector3(0.2f, 0.2f, 0.2f); // Initial scale increase
    public float scaleSpeed = 2f; // Speed of scaling back to original

    private Vector3 originalScale;
    private Vector3 targetScale;
    private bool isScaling = true;

    void Start()
    {
        originalScale = transform.localScale;
        targetScale = originalScale + scaleIncrease;
        // Start a little larger
        transform.localScale = targetScale;
    }

    void OnEnable()
    {
        // Start scaling effect when enabled
        isScaling = true;
    }

    void Update()
    {
        // Rotate the object based on rotationSpeed and time
        transform.Rotate(rotationSpeed * Time.deltaTime);

        // Smoothly scale back to original size
        if (isScaling)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, originalScale, Time.deltaTime * scaleSpeed);

            // Stop scaling when close enough to the original scale
            if (Vector3.Distance(transform.localScale, originalScale) < 0.01f)
            {
                transform.localScale = originalScale;
                isScaling = false;
            }
        }
    }
}
