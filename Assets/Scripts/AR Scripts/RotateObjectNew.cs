using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class RotateObjectNew : MonoBehaviour
{
    public Vector3 rotationSpeed = new Vector3(0, 50, 0); // Rotation speed in degrees per second for each axis
    public Vector3 scaleIncrease = new Vector3(0.3f, 0.3f, 0.3f); // Scale increase when held down
    public float scaleSpeed = 5f; // Speed of scaling up/down
    public GameObject catParent; // Reference to the CatParent GameObject in the Inspector

    private Vector3 targetScale; // Target scale to interpolate towards
    private Vector3 originalScale;
    private bool wasHeld = false; // Track if the object was held down

    public GameObject NameYourCat;
    void Start()
    {
        originalScale = transform.localScale; // Store the original scale of the object
        targetScale = originalScale; // Initialize target scale
    }

    void Update()
    {
        Rotate();

        // Check if the object is being held
        if (IsPressed())
        {
            targetScale = originalScale + scaleIncrease;
            wasHeld = true;
        }
        else
        {
            targetScale = originalScale;

            // If the user releases after holding, enable the CatParent object
            if (wasHeld)
            {
                gameObject.SetActive(false);
                EnableCatParent();
                wasHeld = false; // Reset the flag
            }
        }

        // Smoothly interpolate towards the target scale
        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime * scaleSpeed);
    }

    private void Rotate()
    {
        // Rotate the object based on rotationSpeed and time
        transform.Rotate(rotationSpeed * Time.deltaTime);
    }

    private bool IsPressed()
    {
        // For editor: check if left mouse button is held down
        if (Mouse.current != null && Mouse.current.leftButton.isPressed)
            return true;

        // For mobile: check if there's an active primary touch
        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.isPressed)
            return true;

        return false;
    }

    private void EnableCatParent()
    {
        // Check if catParent is assigned
        if (catParent == null)
        {
            Debug.LogWarning("CatParent GameObject is not assigned.");
            return; // Exit early
        }

        // Get all the child GameObjects
        int childCount = catParent.transform.childCount;
        Debug.Log($"Child count: {childCount}");

        if (childCount > 0)
        {
            // Disable all child cats first
            for (int i = 0; i < childCount; i++)
            {
                catParent.transform.GetChild(i).gameObject.SetActive(false);
            }

            // Retrieve the list of already owned cats
            string ownedCatsString = PlayerPrefs.GetString("userOwnedCats", "");
            List<int> ownedCatIndices = new List<int>();

            if (!string.IsNullOrEmpty(ownedCatsString))
            {
                // Parse the owned cats indices
                foreach (string catIndex in ownedCatsString.Split(','))
                {
                    if (int.TryParse(catIndex, out int index))
                    {
                        ownedCatIndices.Add(index);
                    }
                }
            }

            // Create a list of all possible indices
            List<int> availableIndices = new List<int>();
            for (int i = 0; i < childCount; i++)
            {
                // Only add indices not already owned
                if (!ownedCatIndices.Contains(i))
                {
                    availableIndices.Add(i);
                }
            }

            // Check if there are any cats left to unlock
            if (availableIndices.Count > 0)
            {
                // Pick a random index from the available indices
                int randomIndex = availableIndices[Random.Range(0, availableIndices.Count)];
                Debug.Log($"Random index: {randomIndex}");

                // Enable the randomly selected cat
                catParent.transform.GetChild(randomIndex).gameObject.SetActive(true);

                // Unlock the corresponding cat
                SaveUserOwnedCats(randomIndex);
                BringPanelToFront(NameYourCat);
            }
            else
            {
                Debug.LogWarning("All cats have already been unlocked!");
            }
        }
        else
        {
            Debug.LogWarning("No child cats found in CatParent.");
        }
    }

    public void SaveUserOwnedCats(int index)
    {
        // Retrieve the current list of owned cats
        string ownedCatsString = PlayerPrefs.GetString("userOwnedCats", "");
        List<int> ownedCatIndices = new List<int>();

        if (!string.IsNullOrEmpty(ownedCatsString))
        {
            // Parse the existing indices
            foreach (string catIndex in ownedCatsString.Split(','))
            {
                if (int.TryParse(catIndex, out int parsedIndex))
                {
                    ownedCatIndices.Add(parsedIndex);
                }
            }
        }

        // Add the new index if it's not already owned
        if (!ownedCatIndices.Contains(index))
        {
            ownedCatIndices.Add(index);
        }

        // Save the updated list back to PlayerPrefs
        PlayerPrefs.SetString("userOwnedCats", string.Join(",", ownedCatIndices));
        PlayerPrefs.Save();
    }



    public void BringPanelToFront(GameObject panelToBring)
    {
        panelToBring.transform.SetAsLastSibling(); // Bring to front
    }

}
