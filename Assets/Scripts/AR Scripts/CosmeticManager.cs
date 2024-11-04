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

    private HashSet<Cosmetic> ownedCosmetics = new HashSet<Cosmetic>();
    private List<CatBehavior> selectedCats = new List<CatBehavior>();
    private const string OwnedCosmeticsKey = "userOwnedCosmetics";

    private void Start()
    {
        
        LoadOwnedCosmetics();
    }

    void Update()
    {
        HandleCatSelectionInput();
        UnlockCosmetic(Cosmetic.StarShades);
        ApplyCosmeticToSelected(Cosmetic.StarShades);
    }

    private void HandleCatSelectionInput()
    {
#if UNITY_EDITOR
        HandleMouseInput();
#else
        HandleTouchInput();
#endif
    }

    // Method to handle touch input for selecting cats on mobile devices
    private void HandleTouchInput()
    {
        if (Touchscreen.current != null)
        {
            var touch = Touchscreen.current.primaryTouch;

            if (touch.press.wasPressedThisFrame)
            {
                Vector2 screenPosition = touch.position.ReadValue();
                SelectCat(screenPosition);
            }
        }
    }

    // Method to handle mouse input for selecting cats in the editor
    private void HandleMouseInput()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Vector2 screenPosition = Mouse.current.position.ReadValue();
            SelectCat(screenPosition);
        }
    }

    private void SelectCat(Vector2 screenPosition)
    {
        Ray ray = Camera.main.ScreenPointToRay(screenPosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            CatBehavior cat = hit.collider.GetComponent<CatBehavior>();
            if (cat != null && cat.CanBeSelected()) // Check if the cat can be selected
            {
                if (selectedCats.Contains(cat))
                {
                    selectedCats.Remove(cat); // Deselect if already selected
                    cat.Deselect(); // Visual indication for deselection
                }
                else
                {
                    selectedCats.Add(cat); // Add to selected cats
                    cat.Select(); // Visual indication for selection
                }
            }
        }
    }

    // Unlock a cosmetic and save it in PlayerPrefs
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

    public void ApplyCosmeticToSelected(Cosmetic cosmetic)
    {
        // Check if the cosmetic is owned
        if (!ownedCosmetics.Contains(cosmetic))
        {
            Debug.LogWarning("Cosmetic " + cosmetic + " is not owned.");
            return;
        }

        foreach (CatBehavior cat in selectedCats)
        {
            CatCosmeticHandler cosmeticHandler = cat.GetComponent<CatCosmeticHandler>();
            if (cosmeticHandler != null)
            {
                cosmeticHandler.ApplyCosmetic(cosmetic); // Ensure this accepts the correct type
                Debug.Log("Applied " + cosmetic + " to " + cat.name);
            }
            else
            {
                Debug.LogError("No CatCosmeticHandler found on " + cat.name);
            }
        }

        // Clear selection after applying cosmetic
        selectedCats.Clear();
    }

    // Save owned cosmetics to PlayerPrefs
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

    // Load owned cosmetics from PlayerPrefs
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
}
