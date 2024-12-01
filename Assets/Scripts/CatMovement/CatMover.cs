using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class CatMover : MonoBehaviour
{
    public float moveSpeed = 2f;
    public float rotationSpeed = 5f; // Speed at which the cat rotates to face the target
    public Animator animator; // Reference to the Animator component
    public CatBehavior catBehavior; // Reference to the CatBehavior script (or other scripts controlling interactions)
  
    public bool isWalking = false;

    public void MoveTo(Vector3 targetPosition)
    {
        // Check if the user is interacting with the UI
        if (EventSystem.current.IsPointerOverGameObject())
        {
            // If UI is being interacted with, do not allow movement
            return;
        }

        if (catBehavior.isEating || catBehavior.isDrinking) {
            Debug.Log("Cant move while drinking or eating!");
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
