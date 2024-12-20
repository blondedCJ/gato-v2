using UnityEngine;
using UnityEngine.UI;

public class ToggleButtonsVisibility : MonoBehaviour
{
    // References to the buttons to toggle
    public GameObject button1;
    public GameObject button2;
    public GameObject button3;
    public GameObject button4;
    public GameObject button5;
    public GameObject button6;
    public GameObject button7;
    public GameObject button8;
    public GameObject button9;
    public GameObject button10;
    public GameObject button11;
    public GameObject button12;
    public GameObject button13;
    public GameObject button14;
    public GameObject button15;
    public GameObject button16;
    public GameObject button17; 
    

    public GameObject button21;
    public GameObject button22;
    public GameObject button23;
    public GameObject button24;    
    
    public GameObject button28;
                                 
                                 


    public GameObject button18; // Tier 3
    public GameObject button19; // Tier 1
    public GameObject button20; // Tier 2
 


    // Reference to the toggle button
    public Button toggleButton;

    // Variable to track visibility state
    private bool areButtonsVisible = true;





    // RectTransform for toggle button
    private RectTransform toggleButtonRect;

    // Store original and bottom positions
    private Vector3 originalPosition;
    private Vector3 bottomRightPosition;

    private void Start() {
        // Add listener to the toggle button
        toggleButton.onClick.AddListener(ToggleButtons);



        // Get the RectTransform of the toggle button
        toggleButtonRect = toggleButton.GetComponent<RectTransform>();

        // Store the original position of the toggle button
        originalPosition = toggleButtonRect.anchoredPosition;

        // Define the bottom-right position for AR
        //bottomRightPosition = new Vector3(Screen.width / 2 - 100f, -Screen.height / 2 + 100f, 0); // Adjust the 100f offsets as needed
        bottomRightPosition = new Vector3(-87.80005f, -944, 0);


        // Define the bottom position for AR (adjust as needed)
        //bottomPosition = new Vector3(0, -Screen.height / 2 + 100f, 0); // Adjust the 100f as per your UI requirements

    }

    private void ToggleButtons() {
        areButtonsVisible = !areButtonsVisible;

        // Set active state for the buttons
        button1.gameObject.SetActive(areButtonsVisible);
        button2.gameObject.SetActive(areButtonsVisible);
        button3.gameObject.SetActive(areButtonsVisible);
        button4.gameObject.SetActive(areButtonsVisible);
        button5.gameObject.SetActive(areButtonsVisible);
        button6.gameObject.SetActive(areButtonsVisible);
        button7.gameObject.SetActive(areButtonsVisible);
        button8.gameObject.SetActive(areButtonsVisible);
        button9.gameObject.SetActive(areButtonsVisible);
        button10.gameObject.SetActive(areButtonsVisible);
        button11.gameObject.SetActive(areButtonsVisible);
        button12.gameObject.SetActive(areButtonsVisible);
        button13.gameObject.SetActive(areButtonsVisible);
        button14.gameObject.SetActive(areButtonsVisible);
        button15.gameObject.SetActive(areButtonsVisible);
        button16.gameObject.SetActive(areButtonsVisible); 
        button17.gameObject.SetActive(areButtonsVisible);        
        
        button21.gameObject.SetActive(areButtonsVisible); 
        button22.gameObject.SetActive(areButtonsVisible);  
        button23.gameObject.SetActive(areButtonsVisible);
        
        button24.gameObject.SetActive(areButtonsVisible);
        button28.gameObject.SetActive(areButtonsVisible);

        button19.gameObject.SetActive(areButtonsVisible);
        button20.gameObject.SetActive(areButtonsVisible);
        button18.gameObject.SetActive(areButtonsVisible);



        if (PlayerPrefs.GetInt("GoalsCounter", 0) == 1) {
            button19.gameObject.SetActive(areButtonsVisible);
            button20.gameObject.SetActive(areButtonsVisible);
            button18.gameObject.SetActive(areButtonsVisible);
        }
        if (PlayerPrefs.GetInt("GoalsCounter", 0) == 2) {
            button20.gameObject.SetActive(areButtonsVisible);
            button18.gameObject.SetActive(areButtonsVisible);
        }
        if (PlayerPrefs.GetInt("GoalsCounter", 0) == 3) {
            button18.gameObject.SetActive(areButtonsVisible);
        }

        // Move the toggle button to the bottom or back to its original position
        toggleButtonRect.anchoredPosition = areButtonsVisible ? originalPosition : bottomRightPosition;
    }       
}
