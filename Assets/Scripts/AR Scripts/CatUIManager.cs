using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CatUIManager : MonoBehaviour
{
    public GameObject uiContainer; // Parent GameObject containing all UI elements
    public TMP_Text selectedCatNameText; // UI element to display the selected cat's name
    public Slider hungerSlider; // Reference to the hunger slider
    public Slider affectionSlider; // Reference to the affection slider
    public Slider thirstSlider; // Reference to the thirst slider

    private CatStatus selectedCatStatus; // Reference to the selected cat's status script

    // Cached values for optimization
    private float lastHungerLevel;
    private float lastAffectionLevel;
    private float lastThirstLevel;

    private void Start()
    {
        // Ensure the UI is hidden initially
        uiContainer.SetActive(false);
    }

    public void SetSelectedCat(CatStatus selectedCat)
    {
        // Deselect if the same cat is clicked again
        if (selectedCatStatus == selectedCat)
        {
            DeselectCat();
            return;
        }

        selectedCatStatus = selectedCat;

        if (selectedCatStatus != null)
        {
            // Get the CatNameDisplay component from the selected cat
            CatNameDisplay catNameDisplay = selectedCat.GetComponent<CatNameDisplay>();
            if (catNameDisplay != null)
            {
                // Update the UI with the selected cat's name
                selectedCatNameText.text = catNameDisplay.CatName;
                Debug.Log($"Selected Cat: {catNameDisplay.CatName}");
            }
            else
            {
                selectedCatNameText.text = "Unknown Cat";
                Debug.LogWarning("Selected cat does not have a CatNameDisplay component.");
            }

            // Cache the current status values
            lastHungerLevel = selectedCatStatus.hungerLevel;
            lastAffectionLevel = selectedCatStatus.affectionLevel;
            lastThirstLevel = selectedCatStatus.thirstLevel;

            // Show the UI
            uiContainer.SetActive(true);
            UpdateUI();
        }
    }

    public void DeselectCat()
    {
        // Clear the selected cat and hide the UI
        selectedCatStatus = null;
        uiContainer.SetActive(false);
        Debug.Log("Cat deselected. UI hidden.");
    }

    private void UpdateUI()
    {
        if (selectedCatStatus == null) return;

        // Update the sliders with the selected cat's data
        hungerSlider.value = selectedCatStatus.hungerLevel / 100f;
        affectionSlider.value = selectedCatStatus.affectionLevel / 100f;
        thirstSlider.value = selectedCatStatus.thirstLevel / 100f;
    }

    private void Update()
    {
        // Dynamically update the UI only if the selected cat's status changes
        if (selectedCatStatus != null)
        {
            if (selectedCatStatus.hungerLevel != lastHungerLevel ||
                selectedCatStatus.affectionLevel != lastAffectionLevel ||
                selectedCatStatus.thirstLevel != lastThirstLevel)
            {
                lastHungerLevel = selectedCatStatus.hungerLevel;
                lastAffectionLevel = selectedCatStatus.affectionLevel;
                lastThirstLevel = selectedCatStatus.thirstLevel;

                UpdateUI();
            }
        }
    }
}
