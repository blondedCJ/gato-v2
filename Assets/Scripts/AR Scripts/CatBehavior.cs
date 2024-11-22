using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class CatBehavior : MonoBehaviour
{
    private Animator catAnimator;
    private Collider catCollider;
    private bool isBeingPetted = false;
    private bool isPaused = false;
    private bool isEating = false; // Set this when the cat starts eating
    private bool isDrinking = false; // Set this when the cat starts drinking
    private bool isSelected = false;
    private bool canBeSelected = false; // By default, the cat can't be selected
    private float selectionDelay = 1f;  // Delay period after spawning
    public bool isFinished = false;
    private CatStatus currentlyPettedCat; // Track the specific cat being petted
    private Animator currentCatAnimator;  // Track the specific cat's animator

    public Material selectedMaterial;
    private Material defaultMaterial;
    private SkinnedMeshRenderer catRenderer; // Use SkinnedMeshRenderer for animated models

    private bool canPerformPlayDead = true;
    private bool canPerformJump = true;

    // Animation state names
    private readonly string[] idleStates = {
        "Skeleton_Idle_1_Skeleton", "Skeleton_Idle_2_Skeleton", "Skeleton_Idle_3_Skeleton",
        "Skeleton_Idle_4_Skeleton", "Skeleton_Idle_5_Skeleton", "Skeleton_Idle_6_Skeleton",
        "Skeleton_Idle_7_Skeleton", "Skeleton_Idle_8_Skeleton", "Skeleton_SharpenClaws_Horiz_Skeleton"
    };

    private const string SitStartState = "Skeleton_Sit_start_Skeleton";
    private readonly string[] sitLoopStates = {
        "Skeleton_Sit_loop_1_Skeleton", "Skeleton_Sit_loop_2_Skeleton", "Skeleton_Sit_loop_3_Skeleton" , "Skeleton_Sit_loop_4_Skeleton"// Add more loop states if you have them
    };
    private const string SitEndState = "Skeleton_Sit_end_Skeleton";

    private readonly string[] sleepStartStates = {
        "Skeleton_Lie_back_sleep_start_Skeleton", "Skeleton_Lie_belly_sleep_start_Skeleton",
        "Skeleton_Lie_side_sleep_start_Skeleton"
    };

    private readonly string[] sleepLoopStates = {
        "Skeleton_Lie_back_sleep_Skeleton", "Skeleton_Lie_belly_sleep_Skeleton",
        "Skeleton_Lie_side_sleep_Skeleton"
    };

    private readonly string[] sleepEndStates = {
        "Skeleton_Lie_back_sleep_end_Skeleton", "Skeleton_Lie_belly_sleep_end_Skeleton",
        "Skeleton_Lie_side_sleep_end_Skeleton"
    };

    private const string PettingAnimationState = "Skeleton_Caress_idle_Skeleton"; // Replace with your actual petting animation state
    private bool isDragging = false;

    private enum CatState { Idle, SitStart, SitLoop, SitEnd, SleepStart, SleepLoop, SleepEnd, Eating, PlayDead, Jump }
    private CatState currentState;

    // Track the current sleep start state
    private string currentSleepStartState;
    private Vector2 initialTouchPosition;

    void Start()
    {
        catAnimator = GetComponent<Animator>();
        catCollider = GetComponent<Collider>();
        catRenderer = GetComponentInChildren<SkinnedMeshRenderer>(); // Ensure it looks for the SkinnedMeshRenderer in children
        defaultMaterial = catRenderer.material; // Store the default material
        TransitionToIdle();
        StartCoroutine(EnableSelectionAfterDelay());
    }

    void Update()
    {
        switch (currentState)
        {
            case CatState.Idle:
                if (AnimationFinished(GetCurrentIdleState()))
                {
                    // Randomly choose to sit or sleep after idle
                    if (Random.value > 0.5f)
                        TransitionToSitStart();
                    else
                        TransitionToSleepStart();
                }
                break;

            case CatState.SitStart:
                if (AnimationFinished(SitStartState))
                {
                    TransitionToSitLoop();
                }
                break;

            case CatState.SitLoop:
                // No need to check for animation completion here; handled in the coroutine
                break;

            case CatState.SitEnd:
                // Transition back to Idle after sit end
                if (AnimationFinished(SitEndState))
                {
                    TransitionToIdle();
                }
                break;

            case CatState.SleepStart:
                if (AnimationFinished(currentSleepStartState))
                {
                    TransitionToSleepLoop();
                }
                break;

            case CatState.SleepLoop:
                // No need to check for animation completion here; handled in the coroutine
                break;

            case CatState.SleepEnd:
                // Transition back to Idle after sleep end
                if (AnimationFinished(GetCurrentSleepEndState()))
                {
                    TransitionToIdle();
                }
                break;
        }
        HandlePettingInput();
    }

    private void HandlePettingInput()
    {
#if UNITY_EDITOR
        HandleMouseInput();
#else
        HandleTouchInput();
#endif
    }

    private IEnumerator EnableSelectionAfterDelay()
    {
        yield return new WaitForSeconds(selectionDelay);
        canBeSelected = true; // Allow selection after delay
    }
    public bool CanBeSelected()
    {
        return canBeSelected;
    }

    // Method to handle touch input for mobile devices
    private void HandleTouchInput()
{
    if (Touchscreen.current != null)
    {
        var touch = Touchscreen.current.primaryTouch;

        if (touch.press.wasPressedThisFrame)
        {
            // Register the initial touch position
            initialTouchPosition = touch.position.ReadValue();
        }
        else if (touch.press.isPressed)
        {
            // Check if a drag has occurred
            Vector2 currentPosition = touch.position.ReadValue();
            if (Vector2.Distance(currentPosition, initialTouchPosition) > 10f) // Threshold for dragging
            {
                isDragging = true;
                CheckPettingStart(currentPosition); // Petting starts with drag
            }
        }
        else if (touch.press.wasReleasedThisFrame)
        {
            // End petting when the touch is released
            if (isDragging)
            {
                EndPetting();
                isDragging = false;
            }
        }
    }
}

    // For mouse input (editor testing)
    private void HandleMouseInput()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            // Register the initial mouse position
            initialTouchPosition = Mouse.current.position.ReadValue();
        }
        else if (Mouse.current.leftButton.isPressed)
        {
            // Check if a drag has occurred
            Vector2 currentPosition = Mouse.current.position.ReadValue();
            if (Vector2.Distance(currentPosition, initialTouchPosition) > 10f) // Threshold for dragging
            {
                isDragging = true;
                CheckPettingStart(currentPosition); // Petting starts with drag
            }
        }
        else if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            // End petting when the mouse button is released
            if (isDragging)
            {
                EndPetting();
                isDragging = false;
            }
        }
    }

    // Check if the input position hits the cat's collider
    // Check if the input position hits the cat's collider
    private void CheckPettingStart(Vector2 screenPosition)
    {
        Ray ray = Camera.main.ScreenPointToRay(screenPosition);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            CatStatus catStatus = hit.collider.GetComponent<CatStatus>();
            if (catStatus != null && !isBeingPetted)
            {
                currentlyPettedCat = catStatus; // Store reference to the specific cat being petted
                currentCatAnimator = hit.collider.GetComponent<Animator>(); // Get the animator of the cat being petted
                StartPetting();
            }
        }
    }

    // Start the petting animation and give affection gradually
    private void StartPetting()
    {
        if (!isBeingPetted && currentlyPettedCat != null && currentCatAnimator != null)
        {
            isBeingPetted = true;
            PauseEatingOrDrinking(); // Pause eating or drinking

            // Use the specific cat's animator
            currentCatAnimator.speed = 1;
            currentCatAnimator.CrossFade("Skeleton_Caress_idle_Skeleton", 0.2f); // Play the petting animation

            // Start giving affection gradually
            currentlyPettedCat.StartPettingAffection();
        }
    }

    // End the petting animation and stop affection increase
    private void EndPetting()
    {
        if (isBeingPetted && currentCatAnimator != null)
        {
            isBeingPetted = false;
            ResumeEatingOrDrinking(); // Resume eating or drinking

            // Only transition to idle if not eating or drinking
            if (!isEating && !isDrinking)
            {
                TransitionToIdle(); // Transition back to idle
            }
            else if (isEating)
            {
                currentCatAnimator.CrossFade("Skeleton_Eating_Skeleton", 0.2f);
            }
            else if (isDrinking)
            {
                currentCatAnimator.CrossFade("Skeleton_Drinking_Skeleton", 0.2f);
            }

            // Stop affection increase
            currentlyPettedCat.StopPettingAffection();

            // Clear references when petting ends
            currentlyPettedCat = null;
            currentCatAnimator = null;
        }
    }

    private void PauseEatingOrDrinking()
    {
        if (isEating)
        {
            isPaused = true;
            catAnimator.speed = 0; // Pause the eating animation
            Debug.Log("Eating paused.");
        }
        else if (isDrinking)
        {
            isPaused = true;
            catAnimator.speed = 0; // Pause the drinking animation
            Debug.Log("Drinking paused.");
        }
    }

    private void ResumeEatingOrDrinking()
    {
        if (isEating)
        {
            isPaused = false;
            catAnimator.speed = 1; // Resume the eating animation
            Debug.Log("Eating resumed.");
        }
        else if (isDrinking)
        {
            isPaused = false;
            catAnimator.speed = 1; // Resume the drinking animation
            Debug.Log("Drinking resumed.");
        }
    }

 
    public void TransitionToDrinking(GameObject drinkItem, float duration)
    {
        isDrinking = true;
        currentState = CatState.Eating; // You can change this to a new enum state for drinking if you want
        catAnimator.CrossFade("Skeleton_Drinking_Skeleton", 0.2f); // Replace with the actual drinking animation name

        // Optionally, destroy the drink item after the specified duration
        StartCoroutine(DrinkingCoroutine(drinkItem, duration));
    }

    private IEnumerator DrinkingCoroutine(GameObject drinkItem, float duration)
    {
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            if (!isPaused)
            {
                elapsedTime += Time.deltaTime;
            }

            yield return null;
        }
        Destroy(drinkItem);
        isFinished = true;
        TransitionToIdle();
        isDrinking = false; // Reset drinking flag
    }


    public void TransitionToEating(GameObject foodItem, float duration)
    {   
        isEating = true;
        currentState = CatState.Eating; // Set the state to Eating
        catAnimator.CrossFade("Skeleton_Eating_Skeleton", 0.2f); // Trigger the eating animation

        // Optionally, destroy the food item after the specified duration
        StartCoroutine(EatingCoroutine(foodItem, duration));
    }

    private IEnumerator EatingCoroutine(GameObject foodItem, float duration)
    {
        float elapsedTime = 0f;

        // Continue the loop until the total duration is reached
        while (elapsedTime < duration)
        {
            // If the cat is not being petted, continue the eating process
            if (!isPaused)
            {
                elapsedTime += Time.deltaTime; // Accumulate time only if not paused
            }

            yield return null; // Wait for the next frame
        }

        // Eating completed, destroy the food item
        Destroy(foodItem);
        isFinished = true;

    // Transition the cat back to idle after eating
    TransitionToIdle();
        isEating = false; // Reset eating flag
    }

    private IEnumerator WaitForEatingEnd()
    {
        // Wait for the eating animation to finish
        yield return new WaitForSeconds(catAnimator.GetCurrentAnimatorStateInfo(0).length);

        // Transition back to idle after eating
        TransitionToIdle(); // Return to idle after the eating animation
    }

    public void Select()
    {
        isSelected = true;
    }

    public void Deselect()
    {
        isSelected = false;
    }

    public void TransitionToPlayDead()
    {
        if (!canPerformPlayDead) return;

        canPerformPlayDead = false;
        currentState = CatState.PlayDead;
        catAnimator.CrossFade("Skeleton_Die_R_Skeleton", 0.1f);
        StartCoroutine(WaitForTrickEnd("Skeleton_Die_R_Skeleton"));

        // Start cooldown coroutine
        StartCoroutine(PlayDeadCooldown());
    }

    public void TransitionToJump(float jumpHeight = 0.5f, float jumpDuration = 0.92f)
    {
        if (!canPerformJump) return;

        canPerformJump = false;
        currentState = CatState.Jump;
        catAnimator.CrossFade("Skeleton_Jump_Place_IP_Skeleton", 0.1f);

        // Start the coroutine to handle the jump movement
        StartCoroutine(JumpCoroutine(jumpHeight, jumpDuration));

        // Start cooldown coroutine
        StartCoroutine(JumpCooldown());
    }

    // Coroutine to reset Play Dead cooldown
    private IEnumerator PlayDeadCooldown()
    {
        yield return new WaitForSeconds(5f);
        canPerformPlayDead = true;
    }

    // Coroutine to reset Jump cooldown
    private IEnumerator JumpCooldown()
    {
        yield return new WaitForSeconds(5f);
        canPerformJump = true;
    }

    private void TransitionToIdle()
    {
        // Select a random idle state
        string randomIdleState = idleStates[Random.Range(0, idleStates.Length)];
        catAnimator.CrossFade(randomIdleState, 0.2f);
        currentState = CatState.Idle;
    }

    private void TransitionToSitStart()
    {
        currentState = CatState.SitStart;
        catAnimator.CrossFade(SitStartState, 0.2f);
    }

    private void TransitionToSitLoop()
    {
        currentState = CatState.SitLoop;
        string randomSitLoopState = GetRandomSitLoopState(); // Get a random sit loop state
        catAnimator.CrossFade(randomSitLoopState, 0.2f);
        StartCoroutine(SitLoopCoroutine()); // Start the sit loop coroutine
    }

    private IEnumerator JumpCoroutine(float jumpHeight, float jumpDuration)
    {
        // Calculate the half duration to handle upward and downward movement
        float halfDuration = jumpDuration / 2f;

        Vector3 initialPosition = transform.position; // Store the starting position

        // Optional delay before starting the jump
        yield return new WaitForSeconds(0.2f); // Delay of 0.2 seconds before starting the jump

        // Move the cat upwards for the first half of the duration
        float elapsedTime = 0f;
        while (elapsedTime < halfDuration)
        {
            elapsedTime += Time.deltaTime;
            float newY = Mathf.Lerp(initialPosition.y, initialPosition.y + jumpHeight, elapsedTime / halfDuration);
            transform.position = new Vector3(initialPosition.x, newY, initialPosition.z);
            yield return null;
        }

        // Optional delay at the peak of the jump
        yield return new WaitForSeconds(0.1f); // Delay of 0.1 seconds at the peak of the jump

        // Move the cat back down for the second half of the duration
        elapsedTime = 0f;
        while (elapsedTime < halfDuration)
        {
            elapsedTime += Time.deltaTime;
            float newY = Mathf.Lerp(initialPosition.y + jumpHeight, initialPosition.y, elapsedTime / halfDuration);
            transform.position = new Vector3(initialPosition.x, newY, initialPosition.z);
            yield return null;
        }

        // Ensure the cat returns to the exact starting position
        transform.position = initialPosition;

        // Optional delay after landing before transitioning back to idle
        yield return new WaitForSeconds(0.2f); // Delay of 0.2 seconds after landing

        // Wait for the jump animation to finish before transitioning back to idle
        StartCoroutine(WaitForTrickEnd("Skeleton_Jump_Place_IP_Skeleton"));
    }


    private IEnumerator WaitForTrickEnd(string trickState)
    {
        // Wait for the trick animation to start properly
        yield return new WaitUntil(() => catAnimator.GetCurrentAnimatorStateInfo(0).IsName(trickState));

        // Wait for the trick animation to finish
        yield return new WaitUntil(() => catAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f);

        if (trickState == "Skeleton_Die_R_Skeleton") // For Play Dead
        {
            // Hold the "Play Dead" state for 2 seconds before transitioning
            yield return new WaitForSeconds(10f);
        }
        // For "Jump", it will immediately transition to idle without delay

        // After the trick, transition back to idle
        TransitionToIdle();
    }


    private IEnumerator SitLoopCoroutine()
    {
        // Wait for a random time between 15 and 30 seconds
        float waitTime = Random.Range(15f, 30f);
        yield return new WaitForSeconds(waitTime);
        TransitionToSitEnd(); // Transition to sit end after waiting
    }

    private void TransitionToSitEnd()
    {
        currentState = CatState.SitEnd;
        catAnimator.CrossFade(SitEndState, 0.2f);
        StartCoroutine(WaitForSitEnd());
    }

    private IEnumerator WaitForSitEnd()
    {
        yield return new WaitForSeconds(catAnimator.GetCurrentAnimatorStateInfo(0).length);
        TransitionToIdle();
    }

    private void TransitionToSleepStart()
    {
        currentState = CatState.SleepStart;
        currentSleepStartState = sleepStartStates[Random.Range(0, sleepStartStates.Length)]; // Track the selected sleep start state
        catAnimator.CrossFade(currentSleepStartState, 0.2f);
    }

    private void TransitionToSleepLoop()
    {
        currentState = CatState.SleepLoop;
        string randomSleepLoopState = GetCorrespondingSleepLoopState(currentSleepStartState); // Get the corresponding sleep loop state
        catAnimator.CrossFade(randomSleepLoopState, 0.2f);
        StartCoroutine(SleepLoopCoroutine()); // Start the sleep loop coroutine
    }

    private IEnumerator SleepLoopCoroutine()
    {
        // Wait for a random time between 15 and 30 seconds
        float waitTime = Random.Range(15f, 30f);
        yield return new WaitForSeconds(waitTime);
        TransitionToSleepEnd(); // Transition to sleep end after waiting
    }

    private void TransitionToSleepEnd()
    {
        currentState = CatState.SleepEnd;
        string randomSleepEndState = GetCorrespondingSleepEndState(currentSleepStartState); // Get the corresponding sleep end state
        catAnimator.CrossFade(randomSleepEndState, 0.2f);
        StartCoroutine(WaitForSleepEnd());
    }

    private IEnumerator WaitForSleepEnd()
    {
        yield return new WaitForSeconds(catAnimator.GetCurrentAnimatorStateInfo(0).length);
        TransitionToIdle();
    }

    private bool AnimationFinished(string animationState)
    {
        return catAnimator.GetCurrentAnimatorStateInfo(0).IsName(animationState) &&
               catAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f; // Animation finished
    }

    private string GetCurrentIdleState()
    {
        // Return the current idle state that is playing
        foreach (var state in idleStates)
        {
            if (catAnimator.GetCurrentAnimatorStateInfo(0).IsName(state))
            {
                return state;
            }
        }
        return string.Empty;
    }

    private string GetCurrentSleepEndState()
    {
        // Return the current sleep end state that is playing
        foreach (var state in sleepEndStates)
        {
            if (catAnimator.GetCurrentAnimatorStateInfo(0).IsName(state))
            {
                return state;
            }
        }
        return string.Empty;
    }

    private string GetRandomSitLoopState()
    {
        // Get a random sit loop state
        return sitLoopStates[Random.Range(0, sitLoopStates.Length)];
    }
    
    private string GetCorrespondingSleepLoopState(string sleepStartState)
    {
        // Determine the corresponding sleep loop state based on the sleep start state
        if (sleepStartState == sleepStartStates[0]) // "Skeleton_Lie_back_sleep_start_Skeleton"
            return sleepLoopStates[0]; // "Skeleton_Lie_back_sleep_Skeleton"
        else if (sleepStartState == sleepStartStates[1]) // "Skeleton_Lie_belly_sleep_start_Skeleton"
            return sleepLoopStates[1]; // "Skeleton_Lie_belly_sleep_Skeleton"
        else if (sleepStartState == sleepStartStates[2]) // "Skeleton_Lie_side_sleep_start_Skeleton"
            return sleepLoopStates[2]; // "Skeleton_Lie_side_sleep_Skeleton"

        return string.Empty;
    }

    private string GetCorrespondingSleepEndState(string sleepStartState)
    {
        // Determine the corresponding sleep end state based on the sleep start state
        if (sleepStartState == sleepStartStates[0]) // "Skeleton_Lie_back_sleep_start_Skeleton"
            return sleepEndStates[0]; // "Skeleton_Lie_back_sleep_end_Skeleton"
        else if (sleepStartState == sleepStartStates[1]) // "Skeleton_Lie_belly_sleep_start_Skeleton"
            return sleepEndStates[1]; // "Skeleton_Lie_belly_sleep_end_Skeleton"
        else if (sleepStartState == sleepStartStates[2]) // "Skeleton_Lie_side_sleep_start_Skeleton"
            return sleepEndStates[2]; // "Skeleton_Lie_side_sleep_end_Skeleton"

        return string.Empty;
    }
}
