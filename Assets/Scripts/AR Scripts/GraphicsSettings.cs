using UnityEngine;
using UnityEngine.UI;

public class GraphicsSettingsManager : MonoBehaviour
{
    public Toggle lowToggle;
    public Toggle mediumToggle;
    public Toggle highToggle;
    public ToggleGroup toggleGroup; // Reference to ToggleGroup

    private void Start()
    {
        // Assign each toggle to the ToggleGroup
        lowToggle.group = toggleGroup;
        mediumToggle.group = toggleGroup;
        highToggle.group = toggleGroup;

        // Load and apply saved graphics settings
        LoadGraphicsSettings();
    }

    public void OnLowToggle(bool isOn)
    {
        if (isOn)
        {
            SetGraphicsQuality(0);
        }
    }

    public void OnMediumToggle(bool isOn)
    {
        if (isOn)
        {
            SetGraphicsQuality(1);
        }
    }

    public void OnHighToggle(bool isOn)
    {
        if (isOn)
        {
            SetGraphicsQuality(2);
        }
    }

    private void SetGraphicsQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
        Debug.Log("Current Quality Level: " + QualitySettings.names[QualitySettings.GetQualityLevel()]);
        PlayerPrefs.SetInt("graphicsQuality", qualityIndex);
        PlayerPrefs.Save();
    }

    private void LoadGraphicsSettings()
    {
        // Get the saved quality level, default to Medium (1) if not set
        int savedQuality = PlayerPrefs.GetInt("graphicsQuality", 1);
        QualitySettings.SetQualityLevel(savedQuality);

        // Update toggles to reflect the saved setting
        lowToggle.isOn = savedQuality == 0;
        mediumToggle.isOn = savedQuality == 1;
        highToggle.isOn = savedQuality == 2;
    }
}
