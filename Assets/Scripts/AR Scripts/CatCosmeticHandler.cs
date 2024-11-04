using System;
using System.Collections.Generic;
using UnityEngine;

public class CatCosmeticHandler : MonoBehaviour
{
    [System.Serializable]
    public class CosmeticItem
    {
        public CosmeticManager.Cosmetic cosmetic;
        public GameObject cosmeticObject;
    }

    public List<CosmeticItem> cosmeticItems; // Assign these in the Inspector
    private Dictionary<CosmeticManager.Cosmetic, GameObject> cosmeticDictionary;

    private void Awake()
    {
        // Create the dictionary for easy access
        cosmeticDictionary = new Dictionary<CosmeticManager.Cosmetic, GameObject>();
        foreach (CosmeticItem item in cosmeticItems)
        {
            cosmeticDictionary[item.cosmetic] = item.cosmeticObject;
        }
    }

    private void Start()
    {

    }

    private void OnDestroy()
    {
 
    }

    // Or call SaveCosmetics() when applying a cosmetic
    public void ApplyCosmetic(CosmeticManager.Cosmetic cosmetic)
    {
        if (cosmeticDictionary.TryGetValue(cosmetic, out GameObject cosmeticObject))
        {
            cosmeticObject.SetActive(true); // Enable the cosmetic object
            Debug.Log("Enabled cosmetic: " + cosmetic);
        } else
        {
            Debug.Log("alaaaaa");
        }
    }

    public void SaveCosmetics()
    {
        List<string> enabledCosmetics = new List<string>();

        foreach (var entry in cosmeticDictionary)
        {
            if (entry.Value.activeSelf) // Check if the cosmetic is enabled
            {
                enabledCosmetics.Add(entry.Key.ToString()); // Add the name of the cosmetic
            }
        }

        string key = gameObject.name + "_Cosmetics"; // Unique key for each cat
        PlayerPrefs.SetString(key, string.Join(",", enabledCosmetics)); // Save the cosmetics
        PlayerPrefs.Save(); // Ensure PlayerPrefs are saved
    }
    
    public void LoadCosmetics()
    {
        string key = gameObject.name + "_Cosmetics"; // Unique key for each cat

        if (PlayerPrefs.HasKey(key))
        {
            string cosmeticsString = PlayerPrefs.GetString(key);
            string[] cosmetics = cosmeticsString.Split(',');

            foreach (var cosmetic in cosmetics)
            {
                if (Enum.TryParse(cosmetic, out CosmeticManager.Cosmetic cosmeticEnum))
                {
                    ApplyCosmetic(cosmeticEnum); // Enable the cosmetic
                }
            }
        }
    }


}
