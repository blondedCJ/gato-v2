using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class CosmeticManager : MonoBehaviour
{
    public enum Cosmetic
    {
        JesterHat, MaidHeadband, Bandana, BonnetHat, CaptainHat, CatCap, ClocheHat, DeerHeadband,
        FedoraHat, FlowerHeadband, LeatherHat, OutdoorHat, StrawHat, BambooHat, BandanaHat,
        ElegantHat, FlowerCrown, LatexBeretHat, PirateHat, Prop002_BeanieHat, Prop003_Cap,
        VisorCap, WarriorHelmet, Cube001_0, Cube003_3, StarShades
    }

    public GameObject cosmeticScrollView;

    [Serializable]
    public class CosmeticButtonMapping
    {
        public Cosmetic cosmetic;
        public Button button;
    }

    public List<CosmeticButtonMapping> cosmeticButtonMappings; // List for Inspector

    private Dictionary<Cosmetic, Button> cosmeticButtons = new Dictionary<Cosmetic, Button>();
    private HashSet<Cosmetic> ownedCosmetics = new HashSet<Cosmetic>();
    private Cosmetic? selectedCosmetic = null;

    private const string OwnedCosmeticsKey = "userOwnedCosmetics";

    private void Start()
    {
        // Populate the dictionary with mapped values from the list
        foreach (var mapping in cosmeticButtonMappings)
        {
            cosmeticButtons[mapping.cosmetic] = mapping.button;
        }
        UnlockCosmetic(Cosmetic.JesterHat);
        UnlockCosmetic(Cosmetic.MaidHeadband);
        UnlockCosmetic(Cosmetic.Bandana);
        UnlockCosmetic(Cosmetic.BonnetHat);
        UnlockCosmetic(Cosmetic.CaptainHat);
        UnlockCosmetic(Cosmetic.CatCap);
        UnlockCosmetic(Cosmetic.ClocheHat);
        UnlockCosmetic(Cosmetic.DeerHeadband);
        UnlockCosmetic(Cosmetic.FedoraHat);
        UnlockCosmetic(Cosmetic.FlowerHeadband);
        UnlockCosmetic(Cosmetic.LeatherHat);
        UnlockCosmetic(Cosmetic.OutdoorHat);
        UnlockCosmetic(Cosmetic.StrawHat);
        UnlockCosmetic(Cosmetic.BambooHat);
        UnlockCosmetic(Cosmetic.BandanaHat);
        UnlockCosmetic(Cosmetic.ElegantHat);
        UnlockCosmetic(Cosmetic.FlowerCrown);
        UnlockCosmetic(Cosmetic.LatexBeretHat);
        UnlockCosmetic(Cosmetic.PirateHat);
        UnlockCosmetic(Cosmetic.Prop002_BeanieHat);
        UnlockCosmetic(Cosmetic.Prop003_Cap);
        UnlockCosmetic(Cosmetic.VisorCap);
        UnlockCosmetic(Cosmetic.WarriorHelmet);
        UnlockCosmetic(Cosmetic.Cube001_0);
        UnlockCosmetic(Cosmetic.Cube003_3);
        UnlockCosmetic(Cosmetic.StarShades);
        LoadOwnedCosmetics();
        UpdateCosmeticButtons();

    }

    void Update()
    {
        if (cosmeticScrollView.activeSelf)
        {
            HandleCatSelectionInput();
        } 

    }

    private void HandleCatSelectionInput()
    {
#if UNITY_EDITOR
        HandleMouseInput();
#else
        HandleTouchInput();
#endif
    }

    public void ToggleCosmeticSelectionByIndex(int cosmeticIndex)
    {
        if (!cosmeticScrollView.activeSelf) return;

        Cosmetic cosmetic = (Cosmetic)cosmeticIndex;
        ToggleCosmeticSelection(cosmetic);
    }

    public void ToggleCosmeticSelection(Cosmetic cosmetic)
    {
        if (selectedCosmetic == cosmetic)
        {
            selectedCosmetic = null;
            Debug.Log($"{cosmetic} deselected.");
        }
        else
        {
            selectedCosmetic = cosmetic;
            Debug.Log($"{cosmetic} selected.");
        }
    }

    private void HandleTouchInput()
    {
        if (Touchscreen.current != null && selectedCosmetic.HasValue)
        {
            var touch = Touchscreen.current.primaryTouch;

            if (touch.press.wasPressedThisFrame)
            {
                Vector2 screenPosition = touch.position.ReadValue();
                ToggleCosmeticForCat(screenPosition);
            }
        }
    }

    private void HandleMouseInput()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame && selectedCosmetic.HasValue)
        {
            Vector2 screenPosition = Mouse.current.position.ReadValue();
            ToggleCosmeticForCat(screenPosition);
        }
    }

    private void ToggleCosmeticForCat(Vector2 screenPosition)
    {
        if (selectedCosmetic == null)
        {
            Debug.LogWarning("No cosmetic is currently selected.");
            return;
        }

        Ray ray = Camera.main.ScreenPointToRay(screenPosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            CatBehavior cat = hit.collider.GetComponent<CatBehavior>();
            if (cat != null)
            {
                CatCosmeticHandler cosmeticHandler = cat.GetComponent<CatCosmeticHandler>();
                if (cosmeticHandler != null)
                {
                    if (cosmeticHandler.IsCosmeticApplied((Cosmetic)selectedCosmetic))
                    {
                        cosmeticHandler.RemoveCosmetic((Cosmetic)selectedCosmetic);
                    }
                    else
                    {
                        cosmeticHandler.ApplyCosmetic((Cosmetic)selectedCosmetic);
                    }
                    cosmeticHandler.SaveCosmetics();
                }
            }
        }
    }

    public void UnlockCosmetic(Cosmetic cosmetic)
    {
        if (!ownedCosmetics.Contains(cosmetic))
        {
            ownedCosmetics.Add(cosmetic);
            SaveOwnedCosmetics();
            UpdateCosmeticButtons();
            Debug.Log(cosmetic + " unlocked!");
        }
        else
        {
            Debug.Log(cosmetic + " is already unlocked.");
        }
    }

    private void SaveOwnedCosmetics()
    {
        List<string> ownedCosmeticNames = new List<string>();
        foreach (Cosmetic cosmetic in ownedCosmetics)
        {
            ownedCosmeticNames.Add(cosmetic.ToString());
        }

        string ownedCosmeticsString = string.Join(",", ownedCosmeticNames);
        PlayerPrefs.SetString(OwnedCosmeticsKey, ownedCosmeticsString);
        PlayerPrefs.Save();
    }

    private void LoadOwnedCosmetics()
    {
        if (PlayerPrefs.HasKey(OwnedCosmeticsKey))
        {
            string ownedCosmeticsString = PlayerPrefs.GetString(OwnedCosmeticsKey);
            string[] cosmeticNames = ownedCosmeticsString.Split(',');

            foreach (string cosmeticName in cosmeticNames)
            {
                if (Enum.TryParse(cosmeticName, out Cosmetic cosmetic))
                {
                    ownedCosmetics.Add(cosmetic);
                }
            }
        }
    }

    // Disable cosmetic buttons if cosmetic is not owned
    private void UpdateCosmeticButtons()
    {
        foreach (var cosmeticButtonPair in cosmeticButtons)
        {
            Cosmetic cosmetic = cosmeticButtonPair.Key;
            Button button = cosmeticButtonPair.Value;

            // If the cosmetic is not owned, deactivate the button GameObject (and its children)
            button.gameObject.SetActive(ownedCosmetics.Contains(cosmetic));
        }
    }

    // Call this method when the scroll view is toggled
    public void ClearSelection()
    {
            selectedCosmetic = null;
            Debug.Log("Cleared selected cosmetic as scroll view is hidden.");
    }
}
