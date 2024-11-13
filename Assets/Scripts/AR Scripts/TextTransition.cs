using UnityEngine;
using TMPro;
using System.Collections;

public class TextTransition : MonoBehaviour
{
    public TMP_Text textComponent;       // Reference to the TextMeshPro text
    public string[] paragraphs;          // Array of text paragraphs
    public float fadeDuration = 1.0f;    // Time for fade out and fade in
    public float displayDuration = 3.0f; // Time to display each paragraph

    private CanvasGroup canvasGroup;
    private int lastParagraphIndex = -1; // Keep track of the last paragraph index

    void Start()
    {
        // Get the CanvasGroup component for fading
        canvasGroup = textComponent.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = textComponent.gameObject.AddComponent<CanvasGroup>();
        }

        // Start the paragraph display cycle
        StartCoroutine(DisplayParagraphs());
    }

    private IEnumerator DisplayParagraphs()
    {
        while (true)
        {
            // Select a random paragraph that is not the same as the last one
            int currentParagraphIndex;
            do
            {
                currentParagraphIndex = Random.Range(0, paragraphs.Length);
            } while (currentParagraphIndex == lastParagraphIndex);

            // Update the last index to the current one
            lastParagraphIndex = currentParagraphIndex;

            // Display the selected paragraph
            textComponent.text = paragraphs[currentParagraphIndex];

            // Fade in
            yield return StartCoroutine(FadeIn());

            // Wait for the display duration
            yield return new WaitForSeconds(displayDuration);

            // Fade out
            yield return StartCoroutine(FadeOut());
        }
    }

    private IEnumerator FadeIn()
    {
        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            canvasGroup.alpha = Mathf.Lerp(0, 1, elapsedTime / fadeDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        canvasGroup.alpha = 1;
    }

    private IEnumerator FadeOut()
    {
        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            canvasGroup.alpha = Mathf.Lerp(1, 0, elapsedTime / fadeDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        canvasGroup.alpha = 0;
    }
}
