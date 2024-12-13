using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CatManager : MonoBehaviour
{
    private CatBehavior selectedCat;
    private UserInventory userInventory;

    void Start()
    {
        // Reference to UserInventory component
        userInventory = FindObjectOfType<UserInventory>();

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
            if (cat != null && cat.CanBeSelected())
            {
                if (selectedCat != null && selectedCat == cat)
                {
                    DeselectCat();
                }
                else
                {
                    DeselectCat();
                    selectedCat = cat;
                    cat.Select();
                }
            }
        }
    }

    private void DeselectCat()
    {
        if (selectedCat != null)
        {
            selectedCat.Deselect();
            selectedCat = null;
        }
    }

    public void PerformTrick(string trickName)
    {
        if (selectedCat != null)
        {
            if (selectedCat.isEating || selectedCat.isDrinking || selectedCat.GetComponent<CatMover>().isWalking)
            {
                Debug.Log("Trick is disabled for this cat while it is eating, drinking, or moving.");
                return;
            }

            if (trickName == "PlayDead")
            {
                selectedCat.TransitionToPlayDead();
            }
            else if (trickName == "Jump")
            {
                selectedCat.TransitionToJump();
            }
        }
    }

    public void DisplayUserOwnedCats()
    {
        if (userInventory != null)
        {
            if (userInventory.userOwnedCats.Count > 0)
            {
                Debug.Log("User owns the following cats:");

                foreach (int catIndex in userInventory.userOwnedCats)
                {
                    Debug.Log("Cat index: " + catIndex);
                }
            }
            else
            {
                Debug.Log("User does not own any cats.");
            }
        }
    }

    public void UnlockNewCat(int newCatIndex)
    {
        if (userInventory != null)
        {
            if (!userInventory.userOwnedCats.Contains(newCatIndex))
            {
                userInventory.userOwnedCats.Add(newCatIndex);
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
