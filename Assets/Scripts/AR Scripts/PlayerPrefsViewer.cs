using UnityEngine;

public class PlayerPrefsViewer : MonoBehaviour
{
    private void Start()
    {
        PrintAllPlayerPrefs();
    }

    public void PrintAllPlayerPrefs()
    {
        // Note: Unity does not have a built-in method to directly get all PlayerPrefs keys.
        Debug.Log("PlayerPrefs Viewer:");

        // List all possible keys manually if you know them or use known patterns.
        // Example:
        string[] possibleKeys = { "userOwnedCats", "highScore", "lastLevel", "settingsVolume" }; // Add your known keys here
        foreach (string key in possibleKeys)
        {
            if (PlayerPrefs.HasKey(key))
            {
                Debug.Log($"Key: {key}, Value: {PlayerPrefs.GetString(key)}");
            }
        }

        Debug.Log("End of PlayerPrefs.");
    }
}
