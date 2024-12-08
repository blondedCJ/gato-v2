using UnityEngine;
using TMPro; // Add the TextMeshPro namespace

public class CatCollection : MonoBehaviour
{
    public int lives = 3; // Number of cat's lives
    private int score = 0; // Player's score
    public TMP_Text scoreText; // Reference to the TextMeshPro UI Text component
    public TMP_Text scoreText1; // Reference to the TextMeshPro UI Text component
    private int initialLives; // To store the initial number of lives
    public Spawner spawner;
    public GameObject gameOver;
    public const string CashKey = "PlayerCash";
    //GoalsManager goalsManager;
    //GoalsManager goalsManager;
    [SerializeField] private GoalsManager goalsManager;
    [SerializeField] private GoalsManagerTier2 goalsManagerTier2;
    [SerializeField] private GoalsManagerTier3 goalsManagerTier3;
    private const string GoalsCounterKey = "GoalsCounter";

    void Awake()
    {
        // Store the initial number of lives for reinitialization
        initialLives = lives;


        // Dynamically find GoalsManager if not assigned via Inspector
        if (goalsManager == null) {
            goalsManager = FindObjectOfType<GoalsManager>();
            if (goalsManager == null) {
                Debug.LogError("GoalsManager is not assigned or found!");
            }
        }

        // Dynamically find GoalsManager if not assigned via Inspector
        if (goalsManagerTier2 == null) {
            goalsManagerTier2 = FindObjectOfType<GoalsManagerTier2>();
            if (goalsManagerTier2 == null) {
                Debug.LogError("GoalsManager is not assigned or found!");
            }
        }

        // Dynamically find GoalsManager if not assigned via Inspector
        if (goalsManagerTier3 == null) {
            goalsManagerTier3 = FindObjectOfType<GoalsManagerTier3>();
            if (goalsManagerTier3 == null) {
                Debug.LogError("GoalsManager is not assigned or found!");
            }
        }
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

        if (scoreText1 != null)
        {
            scoreText1.text = $"{score}";
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
        gameOver.SetActive(false);
        Debug.Log("Game Reinitialized!");
    }

    public void onExitSaveCoin()
    {
        int currentCash = PlayerPrefs.GetInt(CashKey, 0);
        currentCash += score;

        PlayerPrefs.SetInt(CashKey, currentCash);
        PlayerPrefs.Save();

        // Goals manager
        if (PlayerPrefs.GetInt(GoalsCounterKey, 0) == 1) {
            goalsManager.IncrementPlayMiniGameGoal();  // Tier 1
            goalsManager.UpdateCashUI();
        }

        if (PlayerPrefs.GetInt(GoalsCounterKey, 0) == 2) {
            goalsManagerTier2.IncrementPlayMiniGameGoal(); //Tier2
            goalsManagerTier2.UpdateCashUI();
        }

        if (PlayerPrefs.GetInt(GoalsCounterKey, 0) == 3) {
            goalsManagerTier3.IncrementPlayMiniGameGoal(); //Tier3
            goalsManagerTier3.UpdateCashUI();
        }
    }

    public void GameOver()
    {
        int currentCash = PlayerPrefs.GetInt(CashKey, 0);
        currentCash += score;

        PlayerPrefs.SetInt(CashKey, currentCash);
        PlayerPrefs.Save();
        //goalsManager.UpdateCashUI();

        Debug.Log("Game Over! You ran out of lives.");

        // Goals manager
        if (PlayerPrefs.GetInt(GoalsCounterKey, 0) == 1) {
            goalsManager.IncrementPlayMiniGameGoal();  // Tier 1
            goalsManager.UpdateCashUI();
        }

        if (PlayerPrefs.GetInt(GoalsCounterKey, 0) == 2) {
            goalsManagerTier2.IncrementPlayMiniGameGoal(); //Tier2
            goalsManagerTier2.UpdateCashUI();
        }

        if (PlayerPrefs.GetInt(GoalsCounterKey, 0) == 3) {
            goalsManagerTier3.IncrementPlayMiniGameGoal(); //Tier3
            goalsManagerTier3.UpdateCashUI();
        }

        Time.timeScale = 0;
        gameOver.SetActive(true);
        // Add game-over logic here (restart, show menu, etc.)
        
    }
}
