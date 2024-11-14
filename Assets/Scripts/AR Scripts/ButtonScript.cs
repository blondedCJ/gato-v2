using UnityEngine;
using UnityEngine.UI;

public class ButtonClickHandler : MonoBehaviour
{
    // This will be set in the inspector for the button's onClick event
    public Button yourButton;

    private void Start()
    {
        // Ensure the button has been set up
        if (yourButton != null)
        {
            // Add a listener for the button click
            yourButton.onClick.AddListener(OnButtonClick);
        }
    }

    // This method will be called when the button is clicked
    private void OnButtonClick()
    {
        // Call the PlayButtonClickSFX method on the AudioManager instance
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayButtonClickSFX(); // Play the click sound
        }
    }
}
