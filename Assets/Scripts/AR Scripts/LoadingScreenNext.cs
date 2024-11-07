using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingScreenNext : MonoBehaviour
{
    public Slider progressBar;       // Assign the slider in the Inspector
    public float fillSpeed = 0.5f;   // Speed at which the progress bar fills, adjust as needed

    private void Start()
    {
        // Start loading the next scene as soon as this scene is loaded
        StartCoroutine(LoadNextSceneAsync());
    }

    private IEnumerator LoadNextSceneAsync()
    {
        int sceneToLoad;

        // Check if the tutorial has been completed
        if (PlayerPrefs.GetInt("GiftSceneCompleted", 0) == 1)
        {
            // If the tutorial is completed, load the main scene
            sceneToLoad = 3;
            Debug.Log("Loading main scene as tutorial is completed.");
        }
        else
        {
            // Otherwise, load the tutorial scene
            sceneToLoad = 2;
            Debug.Log("Loading tutorial scene.");
        }

        // Begin loading the scene asynchronously
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneToLoad);

        // Prevent the scene from activating immediately
        operation.allowSceneActivation = false;

        // Update the progress bar smoothly
        while (!operation.isDone)
        {
            // Calculate the actual loading progress (range 0 to 1)
            float targetProgress = Mathf.Clamp01(operation.progress / 0.9f);

            // Smoothly move the progress bar towards the target progress
            progressBar.value = Mathf.MoveTowards(progressBar.value, targetProgress, fillSpeed * Time.deltaTime);

            // Check if loading is complete
            if (operation.progress >= 0.9f && progressBar.value >= 1f)
            {
                // Scene is fully loaded and progress bar is filled, activate the scene
                operation.allowSceneActivation = true;
            }

            yield return null;
        }
    }
}
