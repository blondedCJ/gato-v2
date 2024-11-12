using UnityEngine;
using TMPro;

public class CatNameDisplay : MonoBehaviour
{
    public TMP_Text catNameText; // Reference to the TMP component for displaying the cat's name
    private static int catCounter = 0; // Static counter to keep track of which cat is being instantiated

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
            // Set the cat name from the userOwnedCatNames array to the TextMeshPro component of this cat
            catNameText.text = ownedCatNames[catCounter];
            Debug.Log($"Cat {catCounter + 1}: {ownedCatNames[catCounter]} displayed in the UI.");

            // Increment the counter so that the next spawned cat gets the next name
            catCounter++;
        }
        else
        {
            Debug.LogWarning("There are more cats instantiated than names available in PlayerPrefs.");
        }
    }
}
