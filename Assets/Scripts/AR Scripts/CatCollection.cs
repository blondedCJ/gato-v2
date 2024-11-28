using UnityEngine;
using TMPro; // Add the TextMeshPro namespace

public class CatCollection : MonoBehaviour
{
    public int lives = 3; // Number of cat's lives
    private int score = 0; // Player's score
    public TMP_Text scoreText; // Reference to the TextMeshPro UI Text component
    private int initialLives; // To store the initial number of lives
    public Spawner spawner;
    public GameObject gameOver;

    void Awake()
    {
        // Store the initial number of lives for reinitialization
        initialLives = lives;
    }

    void OnEnable()
    {
        // Reinitialize the game state when the object is enabled
        Reinitialize();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Food"))
        {
            // Increase score and destroy the food
            score++;
            Debug.Log($"Collected Food! Score: {score}");
            Destroy(other.gameObject);
            UpdateScoreText(); // Update the score text
        }
        else if (other.CompareTag("Dangerous"))
        {
            // Decrease life and destroy the dangerous object
            lives--;
            Debug.Log($"Hit Dangerous Object! Lives left: {lives}");
            Destroy(other.gameObject);

            if (lives <= 0)
            {
                GameOver();
            }
        }
    }

    private void UpdateScoreText()
    {
        // Update the score display
        if (scoreText != null)
        {
            scoreText.text = $"{score}";
        }
    }

    public void Reinitialize()
    {
        // Reset the score and lives
        score = 0;
        lives = initialLives;
        spawner.ClearSpawnedObjects();
        // Update the UI
        UpdateScoreText();
        Time.timeScale = 1;
        Debug.Log("Game Reinitialized!");
    }

    public void GameOver()
    {
        Debug.Log("Game Over! You ran out of lives.");
        Time.timeScale = 0;
        gameOver.SetActive(true);
        // Add game-over logic here (restart, show menu, etc.)
    }
}
