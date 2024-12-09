using UnityEngine;

public class DontDestroyUI : MonoBehaviour
{
    private void Awake()
    {
        // Prevent duplicate instances of the panel
        if (FindObjectsOfType<DontDestroyUI>().Length > 1)
        {
            Destroy(gameObject);
            return;
        }

        // Make the entire panel persistent
        DontDestroyOnLoad(gameObject);
    }
}
