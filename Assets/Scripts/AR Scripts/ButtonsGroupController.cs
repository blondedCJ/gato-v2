using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ButtonsGroupController : MonoBehaviour
{
    [SerializeField]
    private List<Button> buttons;  // List of buttons in the group

    private Button currentlySelectedButton = null;

    private void Start()
    {
        // Add a listener to each button in the list
        foreach (var button in buttons)
        {
            button.onClick.AddListener(() => OnButtonClicked(button));
        }
    }

    private void OnButtonClicked(Button selectedButton)
    {
        if (currentlySelectedButton == selectedButton)
        {
            // If the same button is clicked again, deselect it and enable all buttons
            currentlySelectedButton = null;
            ResetButtons();
        }
        else
        {
            // Select this button and disable the others
            currentlySelectedButton = selectedButton;
            foreach (Button button in buttons)
            {
                button.interactable = (button == selectedButton);
            }
        }
    }

    // Method to reset all buttons to be clickable again
    public void ResetButtons()
    {
        foreach (Button button in buttons)
        {
            button.interactable = true;
        }
    }
}
