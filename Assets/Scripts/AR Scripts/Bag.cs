using UnityEngine;
using UnityEngine.InputSystem;

public class Bag : MonoBehaviour
{
    [SerializeField]
    private GameObject bagObject; // Reference to the bag object itself

    [SerializeField]
    private Animator bagAnimator;

    private bool isBagOpen = false;
    private Vector3 originalScale;
    private Vector3 targetScale;

    [SerializeField]
    private float scaleMultiplier = 1.2f; // Amount to increase size when opened
    [SerializeField]
    private float scaleSpeed = 5f; // Speed of scaling

    // Floating effect variables
    [SerializeField]
    private float floatSpeed = 1f; // Speed of the up and down movement
    [SerializeField]
    private float floatHeight = 0.2f; // Height of the float movement

    private Vector3 initialPosition;

    private void Start()
    {
        if (bagObject == null)
        {
            bagObject = gameObject; // Default to this object if none assigned
        }

        // Store the original scale and position at the start
        originalScale = bagObject.transform.localScale;
        initialPosition = bagObject.transform.position;
        targetScale = originalScale;
    }

    private void Update()
    {
        // Smoothly interpolate the scale of the bag towards the target scale
        bagObject.transform.localScale = Vector3.Lerp(bagObject.transform.localScale, targetScale, Time.deltaTime * scaleSpeed);

        // Floating effect
        float newY = initialPosition.y + Mathf.Sin(Time.time * floatSpeed) * floatHeight;
        bagObject.transform.position = new Vector3(initialPosition.x, newY, initialPosition.z);
    }

    public void Touched()
    {
        if (!isBagOpen)
        {
            OpenBag();
        }
        else
        {
            CloseBag();
        }
    }

    private void OpenBag()
    {
        bagAnimator.SetBool("isClosed", false);
        bagAnimator.SetBool("isOpened", true);
        isBagOpen = true;

        // Set the target scale to the enlarged size
        targetScale = originalScale * scaleMultiplier;
        Debug.Log("Bag is opened!");
    }

    private void CloseBag()
    {
        bagAnimator.SetBool("isOpened", false);
        bagAnimator.SetBool("isClosed", true);
        isBagOpen = false;

        // Set the target scale back to the original size
        targetScale = originalScale;
        Debug.Log("Bag is closed!");
    }
}
