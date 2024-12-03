using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class SceneReset : MonoBehaviour
{
    public ARPlaneManager planeManager; // Reference to the ARPlaneManager

    public void ResetScene()
    {
        // Clear detected planes
        ClearDetectedPlanes();

        // Re-enable ARPlaneManager and restart plane detection
        if (planeManager != null)
        {
            planeManager.enabled = true;
            Debug.Log("Plane scanning re-enabled.");
        }
    }

    private void ClearDetectedPlanes()
    {
        if (planeManager != null)
        {
            // Disable ARPlaneManager to prevent updates while clearing
            planeManager.enabled = false;

            // Destroy all tracked planes
            foreach (var plane in planeManager.trackables)
            {
                if (plane != null && plane.gameObject != null)
                {
                    Destroy(plane.gameObject);
                }
            }

            Debug.Log("Detected planes cleared.");

            // Reset ARSession to ensure no lingering references
            StartCoroutine(ResetARSession());
        }
        else
        {
            Debug.LogWarning("ARPlaneManager is not assigned.");
        }
    }

    private IEnumerator ResetARSession()
    {
        if (ARSession.state > ARSessionState.Ready)
        {
            Debug.Log("Resetting AR session...");

            // Disable the ARSession
            ARSession arSession = FindObjectOfType<ARSession>();
            if (arSession != null)
            {
                arSession.Reset();
            }

            // Allow a small delay for cleanup before re-enabling plane detection
            yield return new WaitForSeconds(1.0f);
        }

        // Re-enable ARPlaneManager for new plane detection
        if (planeManager != null)
        {
            planeManager.enabled = true;
            Debug.Log("ARPlaneManager re-enabled.");
        }
    }
}
