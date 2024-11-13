using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingScreenToMainMenu : MonoBehaviour
{
    public Slider progressBar;       // Assign the slider in the Inspector
    public float fillSpeed = 0.2f;   // Speed at which the progress bar fills, adjust as needed

    private void Start()
    {
        // Start loading the next scene as soon as this scene is loaded
        StartCoroutine(LoadNextSceneAsync());
    }

    private IEnumerator LoadNextSceneAsync()
    {
        // Begin loading the scene asynchronously
        AsyncOperation operation = SceneManager.LoadSceneAsync(2);

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
