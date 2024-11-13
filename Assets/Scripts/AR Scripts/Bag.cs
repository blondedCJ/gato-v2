using System.Collections;
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

    [SerializeField] private RectTransform treatButton; // Assign in Inspector
    [SerializeField] private RectTransform feedButton;  // Assign in Inspector
    [SerializeField] private RectTransform drinkButton; // Assign in Inspector

    private Vector3 treatButtonTargetPosition = new Vector3(327, -444, 0);
    private Vector3 feedButtonTargetPosition = new Vector3(41, -504, 0);
    private Vector3 drinkButtonTargetPosition = new Vector3(-74, -774, 0);

    private Vector3 treatButtonOriginalPosition;
    private Vector3 feedButtonOriginalPosition;
    private Vector3 drinkButtonOriginalPosition;

    [SerializeField] private float buttonMoveDuration = 0.5f; // Speed of button animation

    private void Start()
    {
        if (bagObject == null)
        {
            bagObject = gameObject; // Default to this object if none assigned
        }

        // Store the original scale, initial position, and button positions at the start
        originalScale = bagObject.transform.localScale;
        initialPosition = bagObject.transform.position;
        targetScale = originalScale;

        treatButtonOriginalPosition = treatButton.anchoredPosition;
        feedButtonOriginalPosition = feedButton.anchoredPosition;
        drinkButtonOriginalPosition = drinkButton.anchoredPosition;
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

        // Start coroutine to move buttons to target positions
        StartCoroutine(MoveButtons(treatButton, treatButtonOriginalPosition, treatButtonTargetPosition, buttonMoveDuration));
        StartCoroutine(MoveButtons(feedButton, feedButtonOriginalPosition, feedButtonTargetPosition, buttonMoveDuration));
        StartCoroutine(MoveButtons(drinkButton, drinkButtonOriginalPosition, drinkButtonTargetPosition, buttonMoveDuration));

        Debug.Log("Bag is opened!");
    }

    private void CloseBag()
    {
        bagAnimator.SetBool("isOpened", false);
        bagAnimator.SetBool("isClosed", true);
        isBagOpen = false;

        // Set the target scale back to the original size
        targetScale = originalScale;

        // Start coroutine to move buttons back to their original positions
        StartCoroutine(MoveButtons(treatButton, treatButtonTargetPosition, treatButtonOriginalPosition, buttonMoveDuration));
        StartCoroutine(MoveButtons(feedButton, feedButtonTargetPosition, feedButtonOriginalPosition, buttonMoveDuration));
        StartCoroutine(MoveButtons(drinkButton, drinkButtonTargetPosition, drinkButtonOriginalPosition, buttonMoveDuration));

        Debug.Log("Bag is closed!");
    }

    private IEnumerator MoveButtons(RectTransform button, Vector3 startPosition, Vector3 endPosition, float duration)
    {
        float elapsedTime = 0;

        // Animate the button's position over time using an ease-in-out function
        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;

            // Ease-in-out function (Smooth, non-linear transition)
            float easedT = Mathf.SmoothStep(0f, 1f, t);

            button.anchoredPosition = Vector3.Lerp(startPosition, endPosition, easedT);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure the button reaches its final position
        button.anchoredPosition = endPosition;
    }

}
