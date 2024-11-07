using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using TMPro;

public class LogoScreenNext : MonoBehaviour
{
    public float waitTime = 5f;               // Default time before automatic transition
    public float fadeDuration = 2f;           // Duration of fade effect
    public CanvasGroup fadeCanvasGroup;       // Assign this in the Inspector
    public TextMeshProUGUI breathingText;     // Assign the TextMeshProUGUI text in the Inspector
    public float textBreathingSpeed = 1f;     // Speed of breathing effect for the text

    private bool isTransitioning = false;
    private InputAction tapAction;            // Input action for detecting taps or clicks

    private void Awake()
    {
        // Initialize the InputAction for detecting tap or click
        tapAction = new InputAction(type: InputActionType.Button, binding: "<Pointer>/press");
    }

    private void OnEnable()
    {
        // Enable the tap action
        tapAction.Enable();
        tapAction.performed += OnTapPerformed;
    }

    private void OnDisable()
    {
        // Disable the tap action and unsubscribe the event
        tapAction.performed -= OnTapPerformed;
        tapAction.Disable();
    }

    private void Start()
    {
        // Start the initial fade-in
        StartCoroutine(FadeIn());

        // Start the breathing effect for the text
        StartCoroutine(TextBreathingEffect());

        // Start the automatic scene transition coroutine
        StartCoroutine(WaitAndTransition());
    }

    private void OnTapPerformed(InputAction.CallbackContext context)
    {
        if (!isTransitioning)
        {
            StopAllCoroutines(); // Stop any other coroutines (like the automatic transition)
            StartCoroutine(LoadNextScene());
        }
    }

    private IEnumerator WaitAndTransition()
    {
        // Wait for the specified time before transitioning
        yield return new WaitForSeconds(waitTime);

        // Start the load scene without fade
        yield return LoadNextScene();
    }

    private IEnumerator FadeIn()
    {
        // Fade in at the start
        float timeElapsed = 0f;
        while (timeElapsed < fadeDuration)
        {
            fadeCanvasGroup.alpha = 1 - (timeElapsed / fadeDuration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        // Ensure it's fully transparent
        fadeCanvasGroup.alpha = 0f;
    }

    private IEnumerator LoadNextScene()
    {
        isTransitioning = true;

        // Begin loading the next scene in the background
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(1);
        asyncLoad.allowSceneActivation = false;

        // Wait until the next scene is loaded to 90%
        while (asyncLoad.progress < 0.9f)
        {
            yield return null;
        }

        // Activate the loaded scene to finalize the transition
        asyncLoad.allowSceneActivation = true;
    }

    private IEnumerator TextBreathingEffect()
    {
        // Loop to create a breathing effect
        while (!isTransitioning)
        {
            // Fade in text alpha
            float timeElapsed = 0f;
            while (timeElapsed < textBreathingSpeed)
            {
                float alpha = Mathf.Lerp(0.5f, 1f, timeElapsed / textBreathingSpeed);
                breathingText.alpha = alpha;
                timeElapsed += Time.deltaTime;
                yield return null;
            }

            // Fade out text alpha
            timeElapsed = 0f;
            while (timeElapsed < textBreathingSpeed)
            {
                float alpha = Mathf.Lerp(1f, 0.5f, timeElapsed / textBreathingSpeed);
                breathingText.alpha = alpha;
                timeElapsed += Time.deltaTime;
                yield return null;
            }
        }
    }
}
