using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;

public class ToggleARPlaneVisualization : MonoBehaviour
{
    [SerializeField] private ARPlaneManager arPlaneManager;
    [SerializeField] private Button toggleButton;
    private bool arePlanesVisible = true;

    private void Start()
    {
        // Ensure button is linked
        if (toggleButton != null)
        {
            toggleButton.onClick.AddListener(TogglePlanes);
        }
        else
        {
            Debug.LogError("Toggle Button is not assigned!");
        }

        if (arPlaneManager == null)
        {
            Debug.LogError("ARPlaneManager is not assigned!");
        }
    }

    public void TogglePlanes()
    {
        if (arPlaneManager == null)
        {
            Debug.LogError("ARPlaneManager is missing!");
            return;
        }

        // Toggle plane visibility
        arePlanesVisible = !arePlanesVisible;

        // Enable/Disable all plane GameObjects
        foreach (var plane in arPlaneManager.trackables)
        {
            plane.gameObject.SetActive(arePlanesVisible);
        }

        // Enable/Disable ARPlaneManager
        arPlaneManager.enabled = arePlanesVisible;

        Debug.Log($"AR Plane visualization toggled. Planes visible: {arePlanesVisible}");
    }
}
