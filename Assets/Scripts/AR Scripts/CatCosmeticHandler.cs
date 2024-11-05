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

        // Load previously applied cosmetics
        LoadCosmetics();
    }

    public void ApplyCosmetic(CosmeticManager.Cosmetic cosmetic)
    {
        if (cosmeticDictionary.TryGetValue(cosmetic, out GameObject cosmeticObject))
        {
            cosmeticObject.SetActive(true);
            Debug.Log("Enabled cosmetic: " + cosmetic);
        }
    }

    public void RemoveCosmetic(CosmeticManager.Cosmetic cosmetic)
    {
        if (cosmeticDictionary.TryGetValue(cosmetic, out GameObject cosmeticObject))
        {
            cosmeticObject.SetActive(false);
            Debug.Log("Disabled cosmetic: " + cosmetic);
        }
    }

    public bool IsCosmeticApplied(CosmeticManager.Cosmetic cosmetic)
    {
        return cosmeticDictionary.TryGetValue(cosmetic, out GameObject cosmeticObject) && cosmeticObject.activeSelf;
    }

    public void SaveCosmetics()
    {
        List<string> enabledCosmetics = new List<string>();

        foreach (var entry in cosmeticDictionary)
        {
            if (entry.Value.activeSelf)
            {
                enabledCosmetics.Add(entry.Key.ToString());
            }
        }

        string key = gameObject.name + "_Cosmetics";
        PlayerPrefs.SetString(key, string.Join(",", enabledCosmetics));
        PlayerPrefs.Save();
    }

    public void LoadCosmetics()
    {
        string key = gameObject.name + "_Cosmetics";
        if (PlayerPrefs.HasKey(key))
        {
            string cosmeticsString = PlayerPrefs.GetString(key);
            string[] cosmetics = cosmeticsString.Split(',');

            foreach (var cosmetic in cosmetics)
            {
                if (Enum.TryParse(cosmetic, out CosmeticManager.Cosmetic cosmeticEnum))
                {
                    ApplyCosmetic(cosmeticEnum);
                }
            }
        }
    }
}
