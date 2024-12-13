using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BookTutorialStart : MonoBehaviour
{
    public Image slideshowImage;           // The Image UI element where the images will be displayed
    public Sprite[] images;                // Array of images (sprites) for the slideshow
    public float transitionSpeed = 1f;     // Speed for the fade-in/fade-out transition

    private int currentImageIndex = 0;     // Keeps track of the current image index
    private int counter = 0;               // Counter to track button clicks

    public GameObject closeButton;         // The close button to be activated after 19 clicks
    public GameObject previousButton;  

    // This will be called at the start to initialize the first image
    private void Start() {
        if (images.Length > 0) {
            slideshowImage.sprite = images[currentImageIndex]; // Set the first image in the array
        }

        // Set the close button inactive at the start
        if (closeButton != null) {
            closeButton.SetActive(false);
        }

        // Set the close button inactive at the start
        if (previousButton != null) {
            previousButton.SetActive(false);
        }
    }

    // Method to display the next image
    public void ShowNextImage() {
        currentImageIndex++;

        // Loop back to the first image if we go past the last image
        if (currentImageIndex >= images.Length) {
            currentImageIndex = 0;
        }

        // Increment the counter but cap it at 19
        if (counter < 19) {
            counter++;
        }

        CheckCloseButton(); // Check if the close button should be activated

        StartCoroutine(FadeInImage()); // Start the fade-in effect
    }

    // Method to display the previous image
    public void ShowPreviousImage() {
        currentImageIndex--;

        // Loop back to the last image if we go before the first image
        if (currentImageIndex < 0) {
            currentImageIndex = images.Length - 1;
        }

        // Decrement the counter but don't go below 0 and don't decrement if counter is at 19
        if (counter > 0 && counter < 19) {
            counter--;
        }

        CheckCloseButton(); // Check if the close button should be activated

        StartCoroutine(FadeInImage()); // Start the fade-in effect
    }

    // Coroutine for fading in the new image
    private IEnumerator FadeInImage() {
        float fadeOutDuration = transitionSpeed;
        float timeElapsed = 0f;
        Color currentColor = slideshowImage.color;
        currentColor.a = 1f;
        slideshowImage.color = currentColor;

        // Fade out the current image
        while (timeElapsed < fadeOutDuration) {
            timeElapsed += Time.deltaTime;
            currentColor.a = Mathf.Lerp(1f, 0f, timeElapsed / fadeOutDuration);
            slideshowImage.color = currentColor;
            yield return null;
        }

        // Change to the next image
        slideshowImage.sprite = images[currentImageIndex];

        // Fade in the new image
        timeElapsed = 0f;
        while (timeElapsed < fadeOutDuration) {
            timeElapsed += Time.deltaTime;
            currentColor.a = Mathf.Lerp(0f, 1f, timeElapsed / fadeOutDuration);
            slideshowImage.color = currentColor;
            yield return null;
        }
    }

    // Method to check and activate the close button if the counter reaches or exceeds 19
    private void CheckCloseButton() {
        if (closeButton != null) {
            closeButton.SetActive(counter >= 19);
        }
        if (previousButton != null) {
            previousButton.SetActive(counter >= 19);
        }
    }
}
