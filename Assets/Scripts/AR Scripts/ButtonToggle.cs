using UnityEngine;
using UnityEngine.UI;

public class SpriteToggle : MonoBehaviour
{
    [SerializeField]
    private Image buttonImage;  // Reference to the Image component of the button
    [SerializeField]
    private Sprite sprite1;     // The first sprite
    [SerializeField]
    private Sprite sprite2;     // The second sprite

    private bool isSprite1Active = true;  // Boolean to track which sprite is active

    private void Start()
    {
        if (buttonImage == null)
        {
            buttonImage = GetComponent<Image>();  // Assign the Image component if not already assigned
        }

        // Set initial sprite
        SetSprite(sprite1);
    }

    public void OnButtonClick()
    {
        // Toggle between the sprites
        if (isSprite1Active)
        {
            SetSprite(sprite2);
        }
        else
        {
            SetSprite(sprite1);
        }
    }

    // Public method to reset the button image
    public void ResetToDefault()
    {
        SetSprite(sprite1);
    }

    // Helper method to set the sprite and synchronize the state
    private void SetSprite(Sprite sprite)
    {
        buttonImage.sprite = sprite;
        isSprite1Active = sprite == sprite1;
    }
}
