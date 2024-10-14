using System.Collections;
using UnityEngine;

public class CatBehavior : MonoBehaviour
{
    private Animator catAnimator;

    // Animation state names
    private readonly string[] idleStates = {
        "Skeleton_Idle_1_Skeleton", "Skeleton_Idle_2_Skeleton", "Skeleton_Idle_3_Skeleton",
        "Skeleton_Idle_4_Skeleton", "Skeleton_Idle_5_Skeleton", "Skeleton_Idle_6_Skeleton",
        "Skeleton_Idle_7_Skeleton", "Skeleton_Idle_8_Skeleton", "Skeleton_SharpenClaws_Horiz_Skeleton"
    };

    private const string SitStartState = "Skeleton_Sit_start_Skeleton";
    private readonly string[] sitLoopStates = {
        "Skeleton_Sit_loop_1_Skeleton", "Skeleton_Sit_loop_2_Skeleton" // Add more loop states if you have them
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

    private enum CatState { Idle, SitStart, SitLoop, SitEnd, SleepStart, SleepLoop, SleepEnd }
    private CatState currentState;

    // Track the current sleep start state
    private string currentSleepStartState;

    void Start()
    {
        catAnimator = GetComponent<Animator>();
        TransitionToIdle();
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

    private string GetCurrentSitLoopState()
    {
        // Return the current sit loop state that is playing
        foreach (var state in sitLoopStates)
        {
            if (catAnimator.GetCurrentAnimatorStateInfo(0).IsName(state))
            {
                return state;
            }
        }
        return string.Empty; // Or handle more states if needed
    }

    private string GetCurrentSleepLoopState()
    {
        // Return the current sleep loop state that is playing
        foreach (var state in sleepLoopStates)
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
