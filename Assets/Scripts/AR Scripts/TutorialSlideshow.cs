using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TutorialSlideshow : MonoBehaviour
{
    public Image slideshowImage;       // The Image UI element where the images will be displayed
    public Sprite[] images;            // Array of 9 images (sprites) for the slideshow
    public float transitionSpeed = 1f; // Speed for the fade-in/fade-out transition

    private int currentImageIndex = 0; // Keeps track of the current image index

    // This will be called at the start to initialize the first image
    private void Start() {
        if (images.Length > 0) {
            slideshowImage.sprite = images[currentImageIndex]; // Set the first image in the array
        }
    }

    // Method to display the next image
    public void ShowNextImage() {
        currentImageIndex++;

        // Loop back to the first image if we go past the last image
        if (currentImageIndex >= images.Length) {
            currentImageIndex = 0;
        }

        StartCoroutine(FadeInImage()); // Start the fade-in effect
    }

    // Method to display the previous image
    public void ShowPreviousImage() {
        currentImageIndex--;

        // Loop back to the last image if we go before the first image
        if (currentImageIndex < 0) {
            currentImageIndex = images.Length - 1;
        }

        StartCoroutine(FadeInImage()); // Start the fade-in effect
    }

    // Coroutine for fading in the new image
    private IEnumerator FadeInImage() {
        // Fade out the current image
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
}
