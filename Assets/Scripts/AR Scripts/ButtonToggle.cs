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
        buttonImage.sprite = sprite1;
    }

    public void OnButtonClick()
    {
        // Toggle between the sprites
        if (isSprite1Active)
        {
            buttonImage.sprite = sprite2;
        }
        else
        {
            buttonImage.sprite = sprite1;
        }

        // Toggle the active state
        isSprite1Active = !isSprite1Active;
    }
}
