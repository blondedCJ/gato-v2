using UnityEngine;
using UnityEngine.UI;

public class VolumeSlider : MonoBehaviour
{
    private Slider volumeSlider;

    private void Start()
    {
        volumeSlider = GetComponent<Slider>();

        // Set the slider's initial value to match the saved volume
        volumeSlider.value = PlayerPrefs.GetFloat("musicVolume", 1f);

        // Add listener to update the volume in AudioManager when the slider changes
        volumeSlider.onValueChanged.AddListener(OnVolumeChanged);
    }

    private void OnVolumeChanged(float value)
    {
        if (AudioManager.instance != null)
        {
            AudioManager.instance.ChangeVolume(value);
        }
    }
}
