using System.Collections;
using UnityEngine;

public class CatMovement : MonoBehaviour
{
    public float moveSpeed = 3f; // Movement speed of the cat
    public float turnSpeed = 400f; // Turning speed

    private Animator animator;
    private bool isTurning = false; // Check if the cat is turning
    private bool isMoving = false; // Check if the cat should move
    private string currentAnimation = ""; // To track the current animation

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // Only move the cat if it's supposed to move
        if (isMoving && !isTurning)
        {
            transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
            PlayAnimation("Skeleton_RunFast_F_IP_Skeleton");
        }
        else if (!isTurning) // Play idle animation if not moving or turning
        {
            PlayAnimation("Skeleton_Idle_3_Skeleton 0");
        }
    }

    // Called when Move Left button is pressed
    public void OnMoveLeftPress()
    {
        if (!isTurning)
        {
            Debug.Log($"Current Angle: {transform.eulerAngles.y}, Checking for Left (-90)");
            if (IsFacingDirection(-90)) // Already facing left
            {
                Debug.Log("Already facing left, start moving.");
                isMoving = true; // Start running left
            }
            else
            {
                isMoving = false; // Stop moving during turn
                StartCoroutine(Turn(-180, "Skeleton_Turn180_L_IP_Skeleton"));
            }
        }
    }

    // Called when Move Right button is pressed
    public void OnMoveRightPress()
    {
        if (!isTurning)
        {
            Debug.Log($"Current Angle: {transform.eulerAngles.y}, Checking for Right (90)");
            if (IsFacingDirection(90)) // Already facing right
            {
                Debug.Log("Already facing right, start moving.");
                isMoving = true; // Start running right
            }
            else
            {
                isMoving = false; // Stop moving during turn
                StartCoroutine(Turn(180, "Skeleton_Turn180_R_IP_Skeleton"));
            }
        }
    }

    private IEnumerator Turn(float angle, string turnAnimationName)
    {
        if (isTurning) yield break; // Prevent concurrent turns

        isTurning = true;

        // Transition to the turn animation
        PlayAnimation(turnAnimationName);

        // Calculate the target rotation
        float currentY = transform.eulerAngles.y; // Current Y rotation in degrees (0 to 360)
        float targetY = currentY + angle; // Desired rotation

        // Normalize the target rotation to the range [0, 360)
        targetY = (targetY + 360) % 360;

        // Ensure the shortest path for rotation
        float deltaAngle = Mathf.DeltaAngle(currentY, targetY);

        // Smoothly rotate towards the target
        while (Mathf.Abs(deltaAngle) > 0.1f)
        {
            float rotationStep = Mathf.Sign(deltaAngle) * turnSpeed * Time.deltaTime;
            if (Mathf.Abs(rotationStep) > Mathf.Abs(deltaAngle))
                rotationStep = deltaAngle; // Prevent overshooting

            transform.Rotate(Vector3.up, rotationStep);
            deltaAngle -= rotationStep;

            yield return null;
        }

        // Snap to the exact target rotation
        transform.rotation = Quaternion.Euler(0, targetY, 0);

        isTurning = false;
    }

    public void OnStartMoving()
    {
        isMoving = true; // Set flag to true when movement starts
    }

    public void OnStopMoving()
    {
        isMoving = false; // Set flag to false when movement stops
    }

    private void PlayAnimation(string animationName)
    {
        // Only play the animation if it's not already the current one
        if (currentAnimation != animationName)
        {
            animator.CrossFade(animationName, 0.2f);
            currentAnimation = animationName;
        }
    }

    // Helper function to check if the cat is facing a specific direction
    private bool IsFacingDirection(float targetAngle)
    {
        // Get the forward direction relative to the world's forward axis
        Vector3 forward = transform.forward;
        Vector3 targetDirection = Quaternion.Euler(0, targetAngle, 0) * Vector3.forward;

        // Check if the forward direction aligns closely with the target direction
        return Vector3.Angle(forward, targetDirection) < 10f; // 10-degree tolerance
    }
}
