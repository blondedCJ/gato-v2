using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class SaveNameCat : MonoBehaviour
{
    public TMP_InputField catName;

    public void Proceed()
    {
        string inputText = catName.text;

        // Get the existing names from PlayerPrefs
        string existingNames = PlayerPrefs.GetString("userOwnedCatNames", "");

        // Update the list of names, adding the new name
        string updatedNames = string.IsNullOrEmpty(existingNames) ? inputText : $"{existingNames},{inputText}";

        // Save the updated names back to PlayerPrefs
        PlayerPrefs.SetString("userOwnedCatNames", updatedNames);
        PlayerPrefs.Save();

        // Log that the cat name has been saved
        Debug.Log("Cat name saved!");

        // Print all the existing cat names
        PrintAllCatNames();

        // Complete the cat gift process
        CatGiftComplete();

        // Load the next scene
        SceneManager.LoadScene(3);
    }

    private void PrintAllCatNames()
    {
        // Retrieve the saved cat names from PlayerPrefs
        string existingNames = PlayerPrefs.GetString("userOwnedCatNames", "");

        // If no names are saved, return early
        if (string.IsNullOrEmpty(existingNames))
        {
            Debug.Log("No cats are saved.");
            return;
        }

        // Split the names into an array (or list) using ',' as a separator
        string[] catNames = existingNames.Split(',');

        // Log all existing cat names
        Debug.Log("Existing Cat Names:");
        for (int i = 0; i < catNames.Length; i++)
        {
            // Print the cat index and its corresponding name
            Debug.Log($"Cat {i + 1}: {catNames[i]}");
        }
    }


    public void CatGiftComplete()
    {
        // Save that the tutorial has been completed
        PlayerPrefs.SetInt("GiftSceneCompleted", 1); // Use 1 for true
        PlayerPrefs.Save();
        Debug.Log("GiftScene completed and status saved.");
        // Load the next scene or perform any other action
    }

}

