using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [Header("------------ Audio Source ------------")]
    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioSource SFXSource;

    [Header("------------ Audio Clip ------------")]
    public AudioClip background;
    public AudioClip click;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        musicSource.clip = background;
        musicSource.Play();

        // Set initial volume from PlayerPrefs
        if (!PlayerPrefs.HasKey("musicVolume"))
        {
            PlayerPrefs.SetFloat("musicVolume", 1f);
        }
        AudioListener.volume = PlayerPrefs.GetFloat("musicVolume");
    }

    private void Update()
    {
        AudioListener.volume = PlayerPrefs.GetFloat("musicVolume");
    }

    public void PlayButtonClickSFX()
    {
        // Check if SFXSource is null
        if (SFXSource == null)
        {
            Debug.LogError("SFXSource is not assigned in AudioManager!");
        }

        // Check if the click sound is null
        if (click == null)
        {
            Debug.LogError("Click sound (AudioClip) is not assigned in AudioManager!");
        }

        // Only play the sound if both SFXSource and click are assigned
        if (SFXSource != null && click != null)
        {
            Debug.Log("Playing button click sound...");
            SFXSource.PlayOneShot(click); // Play the button click sound
        }
        else
        {
            Debug.LogWarning("Button click sound not played because one or more references are missing.");
        }
    }


    public void ChangeVolume(float newVolume)
    {
        PlayerPrefs.SetFloat("musicVolume", newVolume);
        PlayerPrefs.Save();
    }
}
