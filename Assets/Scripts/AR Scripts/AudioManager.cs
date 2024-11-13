using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("------------ Audio Source ------------")]
    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioSource SFXSource;

    [Header("------------ Audio Clip ------------")]
    public AudioClip background;

    private void Start()
    {
        // Make this GameObject persist across scenes
        DontDestroyOnLoad(gameObject);

        musicSource.clip = background;
        musicSource.Play();
    }
}
