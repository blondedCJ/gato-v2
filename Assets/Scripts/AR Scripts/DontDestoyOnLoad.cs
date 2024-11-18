using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class ARSceneHandler : MonoBehaviour
{
    private ARSession arSession;

    private void Start() {
        // Find the ARSession in the scene
        arSession = FindObjectOfType<ARSession>();

        if (arSession != null) {
            // Reset AR session to reinitialize tracking and AR components
            arSession.Reset();
        }
    }
}
