using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class UserInputDetector : MonoBehaviour
{
    private InputAction userInputAction;
    [SerializeField]
    private GameObject fullScreenButton; // Button covering the entire screen
    [SerializeField]
    private GameObject touchAnywhere;    // Optional UI overlay or instruction

    private void Awake()
    {
        // Initialize the InputAction for detecting tap or click
        userInputAction = new InputAction(type: InputActionType.Button, binding: "<Keyboard>/anyKey,<Pointer>/press,<Touchscreen>/primaryTouch/press");
    }

    private void OnEnable()
    {
        // Enable the tap action
        userInputAction.Enable();
        userInputAction.performed += OnUserInput;

        // Assign the button click handler if the button is assigned
        if (fullScreenButton != null)
        {
            fullScreenButton.GetComponent<Button>().onClick.AddListener(OnFullScreenButtonClicked);
        }
    }

    private void OnDisable()
    {
        // Disable the tap action and unsubscribe the event
        userInputAction.performed -= OnUserInput;
        userInputAction.Disable();

        // Remove the button click handler
        if (fullScreenButton != null)
        {
            fullScreenButton.GetComponent<Button>().onClick.RemoveListener(OnFullScreenButtonClicked);
        }
    }

    private void Start()
    {
        // Pause the game initially
        Time.timeScale = 0f;

        // Show the full-screen button if assigned
        if (fullScreenButton != null)
        {
            fullScreenButton.SetActive(true);
        }
    }

    private void OnUserInput(InputAction.CallbackContext context)
    {
        Debug.Log($"User input detected from: {context.control.device.name}, control: {context.control.name}");
        UnpauseGame();
    }

    private void OnFullScreenButtonClicked()
    {
        Debug.Log("Full-screen button clicked.");
        UnpauseGame();
    }

    private void UnpauseGame()
    {
        // Unpause the game
        Time.timeScale = 1f;

        // Hide the full-screen button and optional overlay
        if (fullScreenButton != null) fullScreenButton.SetActive(false);
        if (touchAnywhere != null) touchAnywhere.SetActive(false);

        // Disable further input detection
        userInputAction.Disable();
        Debug.Log("InputAction disabled and game unpaused.");
    }

    private void OnDestroy()
    {
        if (userInputAction != null)
        {
            // Unsubscribe and clean up the input action
            userInputAction.performed -= OnUserInput;
            userInputAction.Disable();
            userInputAction.Dispose();
        }

        Debug.Log("InputAction cleaned up.");
    }
}
