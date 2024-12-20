using UnityEngine;
using System.Collections.Generic;

public class UserInventory : MonoBehaviour
{
    public List<int> userOwnedCats { get; private set; } = new List<int>();
    private static List<string> keys = new List<string>();
    public LoadingScreenTest loadingScreenTest;

    // Load user-owned cats from PlayerPrefs
    public void LoadUserOwnedCats()
    {
        userOwnedCats = new List<int>();

        string ownedCatsString = PlayerPrefs.GetString("userOwnedCats", "");
        if (!string.IsNullOrEmpty(ownedCatsString))
        {
            string[] ownedCatsArray = ownedCatsString.Split(',');
            foreach (string catIndex in ownedCatsArray)
            {
                if (int.TryParse(catIndex, out int index))
                {
                    userOwnedCats.Add(index);
                }
            }
        }
    }

    // Optionally, save owned cats to PlayerPrefs
    public void SaveUserOwnedCats(List<int> ownedCats)
    {
        string ownedCatsString = string.Join(",", ownedCats);
        PlayerPrefs.SetString("userOwnedCats", ownedCatsString);
        PlayerPrefs.Save();
    }

    public void ResetUserOwnedCats()
    {
        PlayerPrefs.DeleteAll(); // This will delete all PlayerPrefs data
        PlayerPrefs.Save(); // Ensure changes are saved
        Debug.Log("All PlayerPrefs data has been reset.");
    }
    public static void DisplayAllPlayerPrefs()
    {
        Debug.Log("PlayerPrefs Contents:");
        foreach (var key in keys)
        {
            if (PlayerPrefs.HasKey(key))
            {
                // Display the value based on its type
                string value = key switch
                {
                    var k when PlayerPrefs.GetInt(k) != 0 => PlayerPrefs.GetInt(k).ToString(),
                    var k when PlayerPrefs.GetFloat(k) != 0f => PlayerPrefs.GetFloat(k).ToString(),
                    var k when PlayerPrefs.GetString(k) != "" => PlayerPrefs.GetString(k),
                    _ => "Key has no value"
                };

                Debug.Log($"Key: {key}, Value: {value}");
            }
        }
    }

    public void RemoveSicknessFromAllCats()
    {
        CatStatus[] allCats = FindObjectsOfType<CatStatus>();
        bool anyCatSick = false;

        foreach (CatStatus cat in allCats)
        {
            if (cat.isSick) // Assuming CatStatus has an IsSick() method
            {
                cat.RemoveSickStatus();
                anyCatSick = true; // Set to true if any cat is sick
            }
        }

        // Only start the loading if at least one cat was sick
        if (anyCatSick)
        {
            loadingScreenTest.StartSecondLoading();
        }
        else
        {
            Debug.Log("No cats are sick.");
        }
    }

    public void RemoveDirtyFromAllCats()
    {
        CatStatus[] allCats = FindObjectsOfType<CatStatus>();
        bool anyCatDirty = false;

        foreach (CatStatus cat in allCats)
        {
            if (cat.isDirty) // Assuming CatStatus has an IsDirty() method
            {
                cat.RemoveDirtyStatus();
                anyCatDirty = true; // Set to true if any cat is dirty
            }
        }

        // Only start the loading if at least one cat was dirty
        if (anyCatDirty)
        {
            loadingScreenTest.StartFirstLoading();
        }
        else
        {
            Debug.Log("No cats are dirty.");
        }
    }


}
