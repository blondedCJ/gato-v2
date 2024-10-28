using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingScreenNext : MonoBehaviour
{
    public void NextScene()
    {
        // Check if the tutorial has been completed
        if (PlayerPrefs.GetInt("GiftSceneCompleted", 0) == 1)
        {
            // If the tutorial is completed, do not load it
            Debug.Log("GiftScene has already been completed.");
            // Load the main scene or perform other actions
            SceneManager.LoadScene(3);
        }
        else
        {
            // Load the tutorial scene
            SceneManager.LoadScene(2);
        }
    }
}
