using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class CatBehavior : MonoBehaviour
{
    private Animator catAnimator;
    private Collider catCollider;
    private bool isBeingPetted = false;
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

    public Transform[] heartSpawnPoints; // Array of transforms where hearts will spawn
    public GameObject[] heartPrefabs;    // Array of heart prefabs
    public int heartCount = 5;           // Number of hearts to spawn per pet
    public float spawnInterval = 0.1f;   // Time between each heart spawn
    public float fallSpeed = 2f;         // Speed at which the hearts fall
    public float destroyTime = 3f;       // Time to destroy the hearts after spawning

    private IEnumerator heartSpawnRoutine; // To store the heart spawn coroutine

    // initiate timer slider variables
    public Slider slider;
    public float timer;
    public bool stopTimer = false;

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
        slider.gameObject.SetActive(false);
        slider.enabled = false; 
        TransitionToIdle();
        StartCoroutine(EnableSelectionAfterDelay());

        foreach (GameObject heart in heartPrefabs)
        {
            heart.SetActive(false);

            if (heart.TryGetComponent<Rigidbody>(out Rigidbody rb))
            {
                rb.isKinematic = true; // Disable gravity for original hearts
                Debug.Log("kinematic is true");
            }
        }
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

    // Start continuous heart spawning while petting
    private void StartHeartSpawning()
    {
        if (heartSpawnRoutine != null)
        {
            StopCoroutine(heartSpawnRoutine); // Stop any previous heart spawn coroutine
        }

        heartSpawnRoutine = SpawnHeartsContinuously(); // Start the continuous heart spawning
        StartCoroutine(heartSpawnRoutine);
    }

    // Stop heart spawning when petting ends
    private void StopHeartSpawning()
    {
        if (heartSpawnRoutine != null)
        {
            StopCoroutine(heartSpawnRoutine); // Stop the heart spawning coroutine
            heartSpawnRoutine = null;
        }
        // Optionally, you can clear the hearts or reset their state here
    }

    // Continuous heart spawning while petting
    private IEnumerator SpawnHeartsContinuously()
    {
        while (isBeingPetted) // Continue spawning hearts as long as petting is active
        {
            SpawnHearts(); // Spawn a heart
            yield return new WaitForSeconds(0.5f); // Wait before spawning the next heart (adjust time as needed)
        }
    }

    public void SpawnHearts()
    {
        if (currentlyPettedCat == null)
        {
            Debug.LogWarning("No cat is currently being petted. Cannot spawn hearts.");
            return;
        }

        // Get the spawn points from the currently petted cat
        Transform[] catHeartSpawnPoints = currentlyPettedCat.GetHeartSpawnPoints();

        if (catHeartSpawnPoints == null || catHeartSpawnPoints.Length == 0)
        {
            Debug.LogWarning("No spawn points found for the currently petted cat.");
            return;
        }

        StartCoroutine(SpawnHeartRoutine(catHeartSpawnPoints));
    }

    private IEnumerator SpawnHeartRoutine(Transform[] spawnPoints)
    {
        for (int i = 0; i < heartCount; i++)
        {
            // Randomly select a spawn point and heart prefab
            int randomSpawnIndex = Random.Range(0, spawnPoints.Length);
            Transform spawnPoint = spawnPoints[randomSpawnIndex];

            int randomHeartIndex = Random.Range(0, heartPrefabs.Length);
            GameObject selectedHeart = heartPrefabs[randomHeartIndex];

            // Spawn a heart at the selected spawn point
            GameObject heart = Instantiate(selectedHeart, spawnPoint.position, Quaternion.identity);

            // Set a random scale
            float randomScale = Random.Range(0.05f, 0.15f); // Adjust the min and max sizes as needed
            heart.transform.localScale = new Vector3(randomScale, randomScale, randomScale);

            // Set a random rotation
            heart.transform.rotation = Random.rotation; // Randomizes all axes

            // Ensure the heart is active in the scene
            heart.SetActive(true);

            // Handle Rigidbody or manual movement
            Rigidbody rb = heart.GetComponent<Rigidbody>();

            StartCoroutine(MoveHeartUp(heart)); // Updated to move up

            if (rb != null)
            {
                rb.isKinematic = false; // Enable gravity for the heart
            }
            else
            {
                
            }

            // Destroy the heart after some time
            Destroy(heart, destroyTime);

            // Wait before spawning the next heart
            yield return new WaitForSeconds(spawnInterval);
        }
    }


    private IEnumerator MoveHeartUp(GameObject heart)
    {
        // Define the target position (higher than the current position)
        Vector3 targetPosition = heart.transform.position + Vector3.up * 1f; // Adjust the height as needed

        // Store the starting position
        Vector3 startPosition = heart.transform.position;

        // Set the duration of the float (the time it takes for the heart to reach the target position)
        float floatDuration = Random.Range(1f, 3f); // Randomize float speed for variety
        float timeElapsed = 0f;

        // Floating phase (moving upward)
        while (timeElapsed < floatDuration)
        {
            // Interpolate the position between start and target over time for smooth floating
            heart.transform.position = Vector3.Lerp(startPosition, targetPosition, timeElapsed / floatDuration);

            // Increment time
            timeElapsed += Time.deltaTime;

            yield return null;
        }

        // Ensure it reaches the target position at the end
        heart.transform.position = targetPosition;

        // Fade out the heart after reaching the top
        StartCoroutine(FadeOutHeart(heart));
    }

    private IEnumerator FadeOutHeart(GameObject heart)
    {
        Renderer renderer = heart.GetComponent<Renderer>();

        if (renderer != null && renderer.material.HasProperty("_Color"))
        {
            Color initialColor = renderer.material.color;
            float fadeDuration = 3f; // Time it takes to fully fade out
            float timeElapsed = 0f;

            while (timeElapsed < fadeDuration)
            {
                // Gradually reduce the alpha of the material's color
                float alpha = Mathf.Lerp(initialColor.a, 0f, timeElapsed / fadeDuration);
                renderer.material.color = new Color(initialColor.r, initialColor.g, initialColor.b, alpha);
                timeElapsed += Time.deltaTime;
                yield return null;
            }

            // Ensure the alpha is fully set to 0
            renderer.material.color = new Color(initialColor.r, initialColor.g, initialColor.b, 0f);
        }

        // Destroy the heart object after fading out
        Destroy(heart);
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
            if (Vector2.Distance(currentPosition, initialTouchPosition) > 30f) // Threshold for dragging
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
            if (Vector2.Distance(currentPosition, initialTouchPosition) > 100f) // Threshold for dragging
            {
                isDragging = true;

                // If the cat starts eating or drinking while dragging, end petting
                if (isBeingPetted && (isEating || isDrinking))
                {
                    EndPetting();
                    return;
                }

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
    private void CheckPettingStart(Vector2 screenPosition)
    {
        // Prevent petting if the cat is eating or drinking
        if (isEating || isDrinking)
        {
            Debug.Log("Petting is disabled while the cat is eating or drinking.");
            EndPetting(); // Ensure ongoing petting is stopped
            return;
        }

        Ray ray = Camera.main.ScreenPointToRay(screenPosition);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            CatStatus catStatus = hit.collider.GetComponent<CatStatus>();
            if (catStatus != null && !isBeingPetted)
            {
                currentlyPettedCat = catStatus; // Store reference to the specific cat being petted
                currentCatAnimator = hit.collider.GetComponent<Animator>(); // Get the animator of the cat being petted
                StartPetting();
                StartHeartSpawning(); // Start continuous heart spawning when petting starts
            }
        }
    }


    // Start the petting animation and give affection gradually
    private void StartPetting()
    {
        if (!isBeingPetted && currentlyPettedCat != null && currentCatAnimator != null)
        {
            isBeingPetted = true;
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

            StopHeartSpawning();

            // Stop affection increase
            currentlyPettedCat.StopPettingAffection();

            // Clear references when petting ends
            currentlyPettedCat = null;
            currentCatAnimator = null;
        }
    }

    public void TransitionToDrinking(GameObject drinkItem, float duration)
    {
        //set value slider
        slider.gameObject.SetActive(true);
        slider.enabled = true;
        slider.maxValue = timer;
        slider.value = timer;
        startTimer();

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
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        Destroy(drinkItem);
        slider.gameObject.SetActive(false);
        slider.enabled = false;
        isFinished = true;
        TransitionToIdle();
        isDrinking = false; // Reset drinking flag
    }


    public void TransitionToEating(GameObject foodItem, float duration)
    {
        if (foodItem.ToString() == "CatTreat Variant(Clone) (UnityEngine.GameObject)") {
            timer = 5f;
        } else
        {
            timer = 10f;
        }

        Debug.Log(foodItem.ToString());

        //set value slider
        slider.gameObject.SetActive(true);
        slider.enabled = true;
        slider.maxValue = timer;
        slider.value = timer;
        startTimer();


        isEating = true;
        currentState = CatState.Eating; // Set the state to Eating
        catAnimator.CrossFade("Skeleton_Eating_Skeleton", 0.2f); // Trigger the eating animation

        // Optionally, destroy the food item after the specified duration
        StartCoroutine(EatingCoroutine(foodItem, duration));
    }

    private IEnumerator EatingCoroutine(GameObject foodItem, float duration)
    {
        float elapsedTime = 0f;
        Debug.Log(duration);

        // Continue the loop until the total duration is reached
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime; // Accumulate time only if not paused
            yield return null; // Wait for the next frame
        }

        slider.gameObject.SetActive(false);
        slider.enabled = false;

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


    // timer slider
    public void startTimer() {
        StartCoroutine(StartTimerTicker());

    }
    IEnumerator StartTimerTicker() {
        while (stopTimer == false) {
            timer -= Time.deltaTime;
            yield return new WaitForSeconds(0.001f);
            if (timer <= 0) {
                stopTimer = true;
                timer = 10f;       // Reset timer value
                slider.value = 10; // Reset slider value
            }
            if (stopTimer == false) {
                slider.value = timer;
            }
        }
        // reset boolean
        stopTimer = false;
    }



}


