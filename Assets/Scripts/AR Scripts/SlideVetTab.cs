using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AlternateBringToFront : MonoBehaviour
{
    [Header("UI Buttons")]
    public Button nextButton; // Assign the Next button in the Inspector
    public Button prevButton; // Assign the Next button in the Inspector

    [Header("GameObjects to Swap")]
    public GameObject landia;    // Assign landia GameObject in the Inspector
    public GameObject pawssion;  // Assign pawssion GameObject in the Inspector

    private bool isLandiaActive = true; // To keep track of which GameObject is currently in front

    public GameObject panel;
    private void Start() {

        panel.SetActive(false);

        // Attach the NextButtonClicked method to the button's OnClick event
        if (nextButton != null || prevButton != null) {
            nextButton.onClick.AddListener(NextButtonClicked); 
            prevButton.onClick.AddListener(NextButtonClicked);
        }

        // Ensure landia starts at the front
        if (landia != null) {
            BringGameObjectToFront(landia);
        }
    }
        
    private void NextButtonClicked() {
        if (isLandiaActive) {
            BringGameObjectToFront(pawssion);
        } else {
            BringGameObjectToFront(landia);
        }

        // Toggle the active state for the next click
        isLandiaActive = !isLandiaActive;
    }
    private void BringGameObjectToFront(GameObject obj) {
        if (obj != null) {
            obj.transform.SetAsLastSibling();
        }
    }
}
