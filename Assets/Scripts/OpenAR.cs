using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.Management;
public class OpenAR : MonoBehaviour
{
    private ARSession arSession;

    private void Awake()
    {
        arSession = FindObjectOfType<ARSession>();
    }

    public void GoToNonARScene(int index)
    {
        StartCoroutine(TransitionToNonARScene(index));
    }

    private IEnumerator TransitionToNonARScene(int index)
    {
        if (arSession != null)
        {
            Debug.Log("Resetting AR Session...");
            arSession.Reset();
        }

        if (XRGeneralSettings.Instance.Manager.isInitializationComplete)
        {
            XRGeneralSettings.Instance.Manager.StopSubsystems();
            Debug.Log("Stopped AR Subsystems.");
        }

        yield return null; // Wait for reset

        SceneManager.LoadScene(index);
    }

}
