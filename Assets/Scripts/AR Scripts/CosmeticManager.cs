using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CosmeticManager : MonoBehaviour
{
    public enum Cosmetic
    {
        JesterHat, MaidHeadband, Bandana, BonnetHat, CaptainHat, CatCap, ClocheHat, DeerHeadband,
        FedoraHat, FlowerHeadband, LeatherHat, OutdoorHat, StrawHat, BambooHat, BandanaHat,
        ElegantHat, FlowerCrown, LatexBeretHat, PirateHat, Prop002_BeanieHat, Prop003_Cap,
        VisorCap, WarriorHelmet, Cube001_0, Cube003_3, StarShades
    }

    public GameObject cosmeticScrollView; // Assign this in the Inspector

    private HashSet<Cosmetic> ownedCosmetics = new HashSet<Cosmetic>();
    private Cosmetic? selectedCosmetic = null; // Nullable to check if any cosmetic is selected

    private const string OwnedCosmeticsKey = "userOwnedCosmetics";

    private void Start()
    {
        LoadOwnedCosmetics();
    }

    void Update()
    {
        // Check if the scroll view is active before allowing cat selection
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
                    // Toggle cosmetic on/off and save to PlayerPrefs
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
                if (System.Enum.TryParse(cosmeticName, out Cosmetic cosmetic))
                {
                    ownedCosmetics.Add(cosmetic);
                }
            }
        }
    }

    // Method to clear selection when scroll view is turned off
    public void OnScrollViewToggled(bool isActive)
    {
        if (!isActive)
        {
            selectedCosmetic = null;
            Debug.Log("Cleared selected cosmetic as scroll view is hidden.");
        }
    }
}
