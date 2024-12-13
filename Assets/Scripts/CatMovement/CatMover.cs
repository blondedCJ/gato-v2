using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class CatMover : MonoBehaviour
{
    public float moveSpeed = 2f;
    public float rotationSpeed = 5f; // Speed at which the cat rotates to face the target
    public Animator animator; // Reference to the Animator component
    public CatBehavior catBehavior; // Reference to the CatBehavior script (or other scripts controlling interactions)
    public float yThreshold = 0.1f; // Allowable Y-axis difference for movement

    public bool isWalking = false;

    public bool CanBeSelected { get; private set; } = false;


    private void OnEnable()
    {
        // Start the delay before the cat can be selected
        StartCoroutine(EnableSelectionAfterDelay(1.0f)); // 1 second delay
    }

    private IEnumerator EnableSelectionAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        CanBeSelected = true;
    }

    public void MoveTo(Vector3 targetPosition)
    {
        // Check if the user is interacting with the UI
        if (EventSystem.current.IsPointerOverGameObject())
        {
            Debug.Log("Movement blocked: UI interaction detected.");
            return;
        }

        if (catBehavior.isEating || catBehavior.isDrinking)
        {
            Debug.Log("Cannot move while drinking or eating!");
            return;
        }

        // Validate the target position based on Y-axis
        if (Mathf.Abs(targetPosition.y - transform.position.y) > yThreshold)
        {
            Debug.Log("Movement blocked: Target position is at a different height.");
            return;
        }

        if (!isWalking)
        {
            // Start the movement coroutine
            StartCoroutine(Move(targetPosition));
        }
    }

    private IEnumerator Move(Vector3 target)
    {
        // Set walking state and animation
        isWalking = true;
        animator.CrossFade("Skeleton_Walk_F_IP_Skeleton", 0.2f); // Transition to walking animation

        // Rotate to face the target while moving
        while (Vector3.Distance(transform.position, target) > 0.1f)
        {
            // Rotate to face the target
            Vector3 direction = (target - transform.position).normalized;
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            // Move towards the target
            transform.position = Vector3.MoveTowards(transform.position, target, moveSpeed * Time.deltaTime);

            yield return null;
        }

        // Stop walking and restore idle animation
        isWalking = false;
        catBehavior.TransitionToIdle();
    }
}
