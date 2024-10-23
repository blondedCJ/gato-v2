using UnityEngine;
using System.Collections.Generic;

public class UserInventory : MonoBehaviour
{
    public List<int> userOwnedCats { get; private set; } = new List<int>();
    
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
        userOwnedCats.Clear();
        SaveUserOwnedCats(userOwnedCats);
        Debug.Log("User-owned cats have been reset.");
    }

}
