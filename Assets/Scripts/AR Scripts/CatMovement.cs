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
    private Coroutine turnCoroutine;

    // Timestamp to track the latest input
    private float latestLeftInputTime = 0f;
    private float latestRightInputTime = 0f;

    private enum MovementDirection { None, Left, Right }
    private MovementDirection currentDirection = MovementDirection.None;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // Determine the latest input direction
        MovementDirection priorityDirection = DetermineLatestInputDirection();

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

    private MovementDirection DetermineLatestInputDirection()
    {
        // If times are equal, prefer the last pressed direction
        if (latestLeftInputTime > latestRightInputTime)
            return MovementDirection.Left;
        else if (latestRightInputTime > latestLeftInputTime)
            return MovementDirection.Right;

        return MovementDirection.None;
    }

    private IEnumerator Turn(float angle, string turnAnimationName)
    {
        // Stop any ongoing turn
        if (turnCoroutine != null)
        {
            StopCoroutine(turnCoroutine);
            turnCoroutine = null;
        }

        isTurning = true;

        // Play turn animation
        PlayAnimation(turnAnimationName);

        float currentY = transform.eulerAngles.y;
        float targetY = currentY + angle;
        targetY = (targetY + 360) % 360;

        float deltaAngle = Mathf.DeltaAngle(currentY, targetY);

        while (Mathf.Abs(deltaAngle) > 0.1f)
        {
            float rotationStep = Mathf.Sign(deltaAngle) * turnSpeed * Time.deltaTime;
            if (Mathf.Abs(rotationStep) > Mathf.Abs(deltaAngle))
                rotationStep = deltaAngle;

            transform.Rotate(Vector3.up, rotationStep);
            deltaAngle -= rotationStep;

            yield return null;
        }

        transform.rotation = Quaternion.Euler(0, targetY, 0);

        isTurning = false;
        turnCoroutine = null;
    }

    // Called when Move Left button is pressed
    public void OnMoveLeftPress()
    {
        // Update the latest input timestamp
        latestLeftInputTime = Time.time;
        currentDirection = MovementDirection.Left;

        if (!isTurning)
        {
            if (IsFacingDirection(-90))
            {
                isMoving = true;
            }
            else
            {
                isMoving = false;
                if (turnCoroutine != null) StopCoroutine(turnCoroutine); // Stop current turn
                turnCoroutine = StartCoroutine(Turn(-180, "Skeleton_Turn180_L_IP_Skeleton"));
            }
        }
    }

    // Called when Move Right button is pressed
    public void OnMoveRightPress()
    {
        // Update the latest input timestamp
        latestRightInputTime = Time.time;
        currentDirection = MovementDirection.Right;

        if (!isTurning)
        {
            if (IsFacingDirection(90))
            {
                isMoving = true;
            }
            else
            {
                isMoving = false;
                if (turnCoroutine != null) StopCoroutine(turnCoroutine); // Stop current turn
                turnCoroutine = StartCoroutine(Turn(180, "Skeleton_Turn180_R_IP_Skeleton"));
            }
        }
    }

    // Called when Move Left button is released
    public void OnMoveLeftRelease()
    {
        if (currentDirection == MovementDirection.Left)
        {
            currentDirection = MovementDirection.None;
            isMoving = false;

            // Check if right button is still pressed
            if (latestRightInputTime > latestLeftInputTime)
            {
                OnMoveRightPress();
            }
        }
    }

    // Called when Move Right button is released
    public void OnMoveRightRelease()
    {
        if (currentDirection == MovementDirection.Right)
        {
            currentDirection = MovementDirection.None;
            isMoving = false;

            // Check if left button is still pressed
            if (latestLeftInputTime > latestRightInputTime)
            {
                OnMoveLeftPress();
            }
        }
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