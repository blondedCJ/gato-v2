using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;
public class UIMessageManager : MonoBehaviour
{
    public static UIMessageManager Instance { get; private set; }

    [SerializeField] private TMP_Text messageText; // Assign in the Inspector
    [SerializeField] private float fadeDuration = 1f; // Time for fade-in and fade-out
    [SerializeField] private float displayDuration = 2f; // Time to display the message fully visible

    private Coroutine currentMessageRoutine;

    private void Awake()
    {
        // Singleton Pattern
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Ensure the messageText is initially invisible
        if (messageText != null)
        {
            Color color = messageText.color;
            color.a = 0f; // Fully transparent
            messageText.color = color;
        }
    }

    /// <summary>
    /// Displays a message that fades in, stays visible, and fades out.
    /// </summary>
    /// <param name="message">The message to display.</param>
    public void ShowMessage(string message)
    {
        if (messageText == null)
        {
            Debug.LogError("Message Text component is not assigned.");
            return;
        }

        if (currentMessageRoutine != null)
        {
            StopCoroutine(currentMessageRoutine); // Stop any ongoing message
        }

        currentMessageRoutine = StartCoroutine(DisplayMessageRoutine(message));
    }

    private IEnumerator DisplayMessageRoutine(string message)
    {
        messageText.text = message;

        // Fade In
        yield return FadeText(0f, 1f, fadeDuration);

        // Hold
        yield return new WaitForSeconds(displayDuration);

        // Fade Out
        yield return FadeText(1f, 0f, fadeDuration);
    }

    private IEnumerator FadeText(float startAlpha, float endAlpha, float duration)
    {
        float elapsedTime = 0f;
        Color color = messageText.color;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / duration);
            color.a = alpha;
            messageText.color = color;
            yield return null;
        }

        color.a = endAlpha;
        messageText.color = color;
    }
}
