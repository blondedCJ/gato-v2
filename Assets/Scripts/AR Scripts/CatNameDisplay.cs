using UnityEngine;
using TMPro;

public class CatNameDisplay : MonoBehaviour
{
    public TMP_Text catNameText; // Reference to the TMP component for displaying the cat's name
    private static int catCounter = 0; // Static counter to keep track of which cat is being instantiated
    public string CatName { get; private set; } // Property to store the cat's name

    private void Start()
    {
        // Retrieve the list of owned cat names from PlayerPrefs
        string userOwnedCatNames = PlayerPrefs.GetString("userOwnedCatNames", "");

        // If no cat names are found, log a warning and exit
        if (string.IsNullOrEmpty(userOwnedCatNames))
        {
            Debug.LogWarning("No cat names found in PlayerPrefs.");
            return;
        }

        // Split the names into an array
        string[] ownedCatNames = userOwnedCatNames.Split(',');

        // Ensure we're not out of bounds for the names array
        if (catCounter < ownedCatNames.Length)
        {
            // Set the cat name and display it on this cat
            CatName = ownedCatNames[catCounter];
            catNameText.text = CatName;
            Debug.Log($"Cat {catCounter + 1}: {CatName} displayed in the UI.");

            // Increment the counter for the next spawned cat
            catCounter++;
        }
        else
        {
            Debug.LogWarning("There are more cats instantiated than names available in PlayerPrefs.");
        }
    }

    public static void ResetCatCounter()
    {
        catCounter = 0;
        Debug.Log("Cat counter reset to 0.");
    }

}
