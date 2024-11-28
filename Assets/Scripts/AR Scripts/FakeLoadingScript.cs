using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FakeLoadingScreen : MonoBehaviour
{
    public GameObject loadingScreen; // Assign the loading screen GameObject in the Inspector
    public Slider loadingSlider;     // Assign the Slider UI element in the Inspector

    public void StartLoading(bool isForCleaning)
    {
        if (loadingScreen == null || loadingSlider == null)
        {
            Debug.LogError("Missing references. Ensure LoadingScreen, Slider, and CatStatus are assigned.");
            return;
        }
        loadingScreen.SetActive(true);   // Enable the loading screen
        StartCoroutine(LoadProcess(isForCleaning)); // Start the fake loading coroutine
    }

    private IEnumerator LoadProcess(bool isForCleaning)
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
