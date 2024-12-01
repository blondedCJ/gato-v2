using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FakeLoadingScreen : MonoBehaviour
{

    public void StartLoading(bool isForCleaning, GameObject loadingScreen, Slider loadingSlider)
    {
        if (loadingScreen == null || loadingSlider == null)
        {
            Debug.LogError("Missing references. Ensure LoadingScreen and Slider are assigned.");
            return;
        }

        loadingScreen.SetActive(true);   // Enable the loading screen
        StartCoroutine(LoadProcess(isForCleaning, loadingScreen, loadingSlider)); // Start the fake loading coroutine
    }

    private IEnumerator LoadProcess(bool isForCleaning, GameObject loadingScreen, Slider loadingSlider)
    {
        float loadProgress = 0f;

        // Simulate a loading process
        while (loadProgress < 1f)
        {
            loadProgress += Time.deltaTime / 3f; // 3 seconds to complete loading
            loadingSlider.value = loadProgress; // Update slider value
            yield return null;                  // Wait for the next frame
        }

        // Loading complete
        loadingScreen.SetActive(false); // Disable the loading screen
    }
}
