using UnityEngine;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    [Header("Audio Sources")]
    [SerializeField] private AudioSource musicSource;  // Music audio source
    [SerializeField] private AudioSource sfxSource;    // SFX audio source

    [Header("Button Click SFX")]
    [SerializeField] private AudioClip click; // Button click sound clip
    [Header("Coin Collect SFX")]
    [SerializeField] private AudioClip coin; // Button click sound clip
    [Header("Dangerous Collect SFX")]
    [SerializeField] private AudioClip danger; // Button click sound clip
    [Header("Achievement Unlocked SFX")]
    [SerializeField] private AudioClip achievementUnlocked; // Button click sound clip
    [Header("Cat Unlock SFX")]
    [SerializeField] private AudioClip catUnlocked; // Button click sound clip


    // Singleton instance
    public static AudioManager Instance;

    private void Awake()
    {
        // Ensure there's only one instance of AudioManager
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Persist across scenes
        }
        else
        {
            Destroy(gameObject); // Destroy this instance if one already exists
        }
    }

    private void Start()
    {
        // Load saved volume settings
        LoadVolumeSettings();
    }

    public void SetSliders(Slider music, Slider sfx)
    {
        music.onValueChanged.AddListener(OnMusicVolumeChanged);
        sfx.onValueChanged.AddListener(OnSFXVolumeChanged);

        // Load the saved settings for the sliders
        music.value = PlayerPrefs.GetFloat("musicVolume", 0.1f);
        sfx.value = PlayerPrefs.GetFloat("sfxVolume", 1f);

        // Set the initial volume for the audio sources
        musicSource.volume = music.value;
        sfxSource.volume = sfx.value;
    }

    // Play the button click SFX
    public void PlayButtonClickSFX()
    {
        if (sfxSource != null && click != null)
        {
            sfxSource.PlayOneShot(click); // Play the button click sound
        }
    }

    public void PlayCoinCollectSFX()
    {
        if (sfxSource != null && click != null)
        {
            sfxSource.PlayOneShot(coin); // Play the button click sound
        }
    }

    public void PlayDangerousCollectSFX()
    {
        if (sfxSource != null && click != null)
        {
            sfxSource.PlayOneShot(danger); // Play the button click sound
        }
    }

    public void PlayAchievementUnlocked()
    {
        if (sfxSource != null && click != null)
        {
            sfxSource.PlayOneShot(achievementUnlocked); // Play the button click sound
        }
    }

    public void PlayCatUnlocked()
    {
        if (sfxSource != null && click != null)
        {
            sfxSource.PlayOneShot(catUnlocked); // Play the button click sound
        }
    }
    


    private void OnMusicVolumeChanged(float value)
    {
        musicSource.volume = value;
        SaveVolumeSettings();
    }

    private void OnSFXVolumeChanged(float value)
    {
        sfxSource.volume = value;
        SaveVolumeSettings();
    }

    // Save volume settings in PlayerPrefs
    private void SaveVolumeSettings()
    {
        PlayerPrefs.SetFloat("musicVolume", musicSource.volume);
        PlayerPrefs.SetFloat("sfxVolume", sfxSource.volume);
        PlayerPrefs.Save();
    }

    // Load volume settings from PlayerPrefs
    private void LoadVolumeSettings()
    {
        if (PlayerPrefs.HasKey("musicVolume"))
        {
            musicSource.volume = PlayerPrefs.GetFloat("musicVolume");
        }
        else
        {
            musicSource.volume = 0.1f;  // Default music volume
        }

        if (PlayerPrefs.HasKey("sfxVolume"))
        {
            sfxSource.volume = PlayerPrefs.GetFloat("sfxVolume");
        }
        else
        {
            sfxSource.volume = 1f;  // Default SFX volume
        }
    }

    private void OnDestroy()
    {
        // Clean up listeners
    }
}
