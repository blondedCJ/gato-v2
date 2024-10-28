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

        string existingNames = PlayerPrefs.GetString("userOwnedCatNames", "");
        string updatedNames = string.IsNullOrEmpty(existingNames) ? inputText : $"{existingNames},{inputText}";
        PlayerPrefs.SetString("userOwnedCatNames", updatedNames);
        PlayerPrefs.Save();
        Debug.Log("Cat name saved!");
        CatGiftComplete();
        SceneManager.LoadScene(3);
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

