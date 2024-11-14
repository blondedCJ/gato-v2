using UnityEngine;
using UnityEngine.UI;

public class ButtonClickSFX : MonoBehaviour
{
    private AudioManager audioManager;

    void Start()
    {
        // Find the AudioManager across scenes
        audioManager = FindObjectOfType<AudioManager>();

        // Ensure the AudioManager is found
        if (audioManager != null)
        {
            Button button = GetComponent<Button>();
            button.onClick.AddListener(() => audioManager.PlayButtonClickSFX());
        }
        else
        {
            Debug.LogError("AudioManager not found!");
        }
    }
}
