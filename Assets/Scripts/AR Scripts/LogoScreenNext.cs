using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LogoScreenNext : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // Start the coroutine
        StartCoroutine(TransitionToNextScene());
    }

    // Coroutine to wait for 2 seconds and transition to the next scene
    private IEnumerator TransitionToNextScene()
    {
        // Wait for 2 seconds
        yield return new WaitForSeconds(2f);

        // Load the next scene
        SceneManager.LoadScene(1);
    }
}
