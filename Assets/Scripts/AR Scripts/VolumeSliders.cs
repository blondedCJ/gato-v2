using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    [Header("Sliders")]
    [SerializeField] private Slider musicSlider;  // Music volume slider
    [SerializeField] private Slider sfxSlider;    // SFX volume slider

    private void Start()
    {
        // Find the AudioManager and set the sliders
        AudioManager audioManager = FindObjectOfType<AudioManager>();
        if (audioManager != null)
        {
            audioManager.SetSliders(musicSlider, sfxSlider);
        }
    }
}
