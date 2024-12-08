using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class CatManager : MonoBehaviour
{
    public List<CatBehavior> selectedCats = new List<CatBehavior>();
    private Collider catCollider;
    private UserInventory userInventory;
    private Outline outline; // Reference to the outline component

    private void Awake()
    {
        if (outline != null)
        {
            outline.enabled = false; // Make sure outline is off initially
        }
    }

    void Start()
    {
        // Reference to UserInventory component
        userInventory = FindObjectOfType<UserInventory>();
        // Ensure the user-owned cats are loaded when the game starts


        if (userInventory != null)
        {
            userInventory.LoadUserOwnedCats();
            DisplayUserOwnedCats();
        }
    }

    void Update()
    {
        HandleCatSelectionInput();
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
                }
                else
                {
                    selectedCats.Add(cat); // Add to selected cats
                    cat.Select(); // You can add a visual indication here
                }
            }
        }
    }

    // Method to make all selected cats perform a trick
    public void PerformTrick(string trickName)
    {
        foreach (CatBehavior cat in selectedCats)
        {
            // Check if the cat is eating, drinking, or moving
            if (cat.isEating || cat.isDrinking || cat.GetComponent<CatMover>().isWalking)
            {
                Debug.Log("Trick is disabled for this cat while it is eating, drinking, or moving.");
                continue; // Skip this cat and move to the next one
            }

            // Perform the trick if the cat is not eating, drinking, or moving
            if (trickName == "PlayDead")
            {
                cat.TransitionToPlayDead();
              
            }
            else if (trickName == "Jump")
            {
                cat.TransitionToJump();
            }
            // Add more trick conditions as needed
        }
    }


    // Method to display all owned cats
    public void DisplayUserOwnedCats()
    {
        if (userInventory != null)
        {
            if (userInventory.userOwnedCats.Count > 0)
            {
                Debug.Log("User owns the following cats:");

                // Loop through each cat index and display the owned cats
                foreach (int catIndex in userInventory.userOwnedCats)
                {
                    Debug.Log("Cat index: " + catIndex);
                    // Optionally, you can add logic to display the actual cat names or prefabs
                }
            }
            else
            {
                Debug.Log("User does not own any cats.");
            }
        }
    }

    private void LogSelectedCats()
    {
        if (selectedCats.Count > 0)
        {
            Debug.Log("Selected Cats:");
            foreach (CatBehavior cat in selectedCats)
            {
                // Assuming CatBehavior has a property called CatName (or similar)
                Debug.Log($"- {cat.gameObject.name}"); // Log the name of the cat GameObject
            }
        }
        else
        {
            Debug.Log("No cats are currently selected.");
        }
    }

    public void ClearSelection()
    {
        foreach (CatBehavior cat in selectedCats)
        {
            cat.Deselect(); // Visual indication of deselection
        }
        selectedCats.Clear(); // Clear the selection
    }

    public void UnlockNewCat(int newCatIndex)
    {
        // Check if userInventory is available
        if (userInventory != null)
        {
            // Add the new cat to the user-owned list, if it isn't already there
            if (!userInventory.userOwnedCats.Contains(newCatIndex))
            {
                userInventory.userOwnedCats.Add(newCatIndex);

                // Save the updated list of owned cats
                userInventory.SaveUserOwnedCats(userInventory.userOwnedCats);

                Debug.Log("New cat unlocked and saved!");
            }
            else
            {
                Debug.Log("User already owns this cat.");
            }
        }
    }
}
