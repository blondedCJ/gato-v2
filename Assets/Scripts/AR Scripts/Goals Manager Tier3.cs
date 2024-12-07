using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GoalsManagerTier3 : MonoBehaviour
{

    public static GoalsManagerTier3 Instance { get; private set; }

    public const string CashKey = "PlayerCash";

    // Define PlayerPrefs keys for goal progress and achievements
    private const string TreatsGoalKey = "TreatsGoalProgress3";
    public const string TreatsGoalAchievedKey = "TreatsGoalAchieved3";

    // Cash rewards for each goal
    private const int TreatsGoalCashReward = 60;

    // Goal limits
    private const int TreatsGoalLimit = 25;

    // Goal limits for banner
    private const int BannerGoalLimit = 5;

    private int treatsGiven = 0;

    public TMP_Text treatsProgressText;

    public TMP_Text cashBalanceText;  // Text UI for displaying cash balance

    public Image treatsAchievementImage;

    public Sprite achievedSprite;

    public Slider goalsCompleted; // The slider representing goal completion
    public TMP_Text goalsCompletedText; // The text showing the number of completed goals



    private void Start() {
        //PlayerPrefs.DeleteAll(); // Clears all saved PlayerPrefs data
        //PlayerPrefs.Save();      // Force save the cleared data
        //PlayerPrefs.SetInt(CashKey, 1000);
        //PlayerPrefs.Save();


        LoadGoalsProgress();
        LoadAchievementsStatus();
        UpdateUI();
        UpdateCashUI(); // Update cash display on startup

        // Load the slider and text progress
        goalsCompleted.value = PlayerPrefs.GetFloat("GoalsSliderValue3", 0f);
        goalsCompletedText.text = PlayerPrefs.GetInt("GoalsCompletedCount3", 0).ToString();
    }


    public void IncrementTreatsGoal() {
        if (treatsGiven < TreatsGoalLimit) {
            treatsGiven++;
            SaveGoalProgress(TreatsGoalKey, treatsGiven);
            CheckGoalCompletion();
            UpdateUI();
        }
    }


    // Mini Game
    private const string MiniGameGoalKey = "MiniGameGoalProgress3";
    public const string MiniGameAchievedKey = "MiniGameGoalAchieved3";
    private const int MiniGameGoalCashReward = 210;
    public TMP_Text MiniGameProgressText;
    public Image MiniGameAchievementImage;
    int miniGamePlayed;
    int miniGamePlayedLimit = 30;
    public void IncrementPlayMiniGameGoal() {
        if (miniGamePlayed < miniGamePlayedLimit) {
            miniGamePlayed++;
            SaveGoalProgress(MiniGameGoalKey, miniGamePlayed);
            CheckGoalCompletion();
            UpdateUI();
        }
    }

    // Perform trick
    private const string TrickGoalKey = "TrickGoalProgress3";
    public const string TrickAchievedKey = "TrickGoalAchieved3";
    private const int TrickGoalCashReward = 210;
    public TMP_Text TrickProgressText;
    public Image TrickAchievementImage;
    int TrickGiven;
    int TrickLimit = 25;
    public void IncrementPerformTrickGoal() {
        if (TrickGiven < TrickLimit) {
            TrickGiven++;
            SaveGoalProgress(TrickGoalKey, TrickGiven);
            CheckGoalCompletion();
            UpdateUI();
        }

    }

    // Bathing
    private const string BathGoalKey = "BathGoalProgress3";
    public const string BathAchievedKey = "BathGoalAchieved3";
    private const int BathGoalCashReward = 100;
    public TMP_Text BathProgressText;
    public Image BathAchievementImage;
    int BathGiven;
    int BathLimit = 20;
    public void IncrementBathGoal() {

        if (BathGiven < BathLimit) {
            BathGiven++;
            SaveGoalProgress(BathGoalKey, BathGiven);
            CheckGoalCompletion();
            UpdateUI();
        }

    }

    public void IncrementUnlockAccessories() {

    }

    private void SaveGoalProgress(string key, int progress) {
        PlayerPrefs.SetInt(key, progress);
        PlayerPrefs.Save();
    }

    private void LoadGoalsProgress() {
        treatsGiven = PlayerPrefs.GetInt(TreatsGoalKey, 0);
        miniGamePlayed = PlayerPrefs.GetInt(MiniGameGoalKey, 0);
        TrickGiven = PlayerPrefs.GetInt(TrickGoalKey, 0);
        BathGiven = PlayerPrefs.GetInt(BathGoalKey, 0);
    }

    private void LoadAchievementsStatus() {
        //treats
        if ((PlayerPrefs.GetInt(TreatsGoalAchievedKey, 0) == 1
            || PlayerPrefs.GetInt(TreatsGoalAchievedKey, 0) == 2)
            && achievedSprite != null)
            treatsAchievementImage.sprite = achievedSprite;

        //minigame
        if ((PlayerPrefs.GetInt(MiniGameAchievedKey, 0) == 1
            || PlayerPrefs.GetInt(MiniGameAchievedKey, 0) == 2)
            && achievedSprite != null)
            MiniGameAchievementImage.sprite = achievedSprite;

        //trick
        if ((PlayerPrefs.GetInt(TrickAchievedKey, 0) == 1
            || PlayerPrefs.GetInt(TrickAchievedKey, 0) == 2)
            && achievedSprite != null)
            TrickAchievementImage.sprite = achievedSprite;

        //bathing
        if ((PlayerPrefs.GetInt(BathAchievedKey, 0) == 1
            || PlayerPrefs.GetInt(BathAchievedKey, 0) == 2)
            && achievedSprite != null)
            BathAchievementImage.sprite = achievedSprite;
    }


    private void CheckGoalCompletion() {
        bool goalAchieved = false;

        //treats
        if (treatsGiven >= TreatsGoalLimit && PlayerPrefs.GetInt(TreatsGoalAchievedKey, 0) == 0) {
            treatsAchievementImage.sprite = achievedSprite;
            PlayerPrefs.SetInt(TreatsGoalAchievedKey, 1);
            goalAchieved = true;
        }

        //minigame
        if (miniGamePlayed >= miniGamePlayedLimit && PlayerPrefs.GetInt(MiniGameAchievedKey, 0) == 0) {
            MiniGameAchievementImage.sprite = achievedSprite;
            PlayerPrefs.SetInt(MiniGameAchievedKey, 1);
            goalAchieved = true;
        }

        // perform tricks
        if (TrickGiven >= TrickLimit && PlayerPrefs.GetInt(TrickAchievedKey, 0) == 0) {
            TrickAchievementImage.sprite = achievedSprite;
            PlayerPrefs.SetInt(TrickAchievedKey, 1);
            goalAchieved = true;
        }

        // bathing 
        if (BathGiven >= BathLimit && PlayerPrefs.GetInt(BathAchievedKey, 0) == 0) {
            BathAchievementImage.sprite = achievedSprite;
            PlayerPrefs.SetInt(BathAchievedKey, 1);
            goalAchieved = true;
        }

        // Update the slider and text if a goal was achieved
        if (goalAchieved) {
            IncrementGoalsCompleted();
        }

        PlayerPrefs.Save();
    }


    public void ClaimReward(string goalKey) {
        int cashReward = 0;

        switch (goalKey) {
            case TreatsGoalAchievedKey:
                if (PlayerPrefs.GetInt(TreatsGoalAchievedKey, 0) == 1)
                    cashReward = TreatsGoalCashReward;
                break;

            case MiniGameAchievedKey:
                if (PlayerPrefs.GetInt(MiniGameAchievedKey, 0) == 1)
                    cashReward = MiniGameGoalCashReward;
                break;

            case TrickAchievedKey:
                if (PlayerPrefs.GetInt(TrickAchievedKey, 0) == 1)
                    cashReward = TrickGoalCashReward;
                break;

            case BathAchievedKey:
                if (PlayerPrefs.GetInt(BathAchievedKey, 0) == 1)
                    cashReward = BathGoalCashReward;
                break;

        }

        if (cashReward > 0) {
            int currentCash = PlayerPrefs.GetInt(CashKey, 0);
            currentCash += cashReward;
            PlayerPrefs.SetInt(CashKey, currentCash);
            PlayerPrefs.Save();

            UpdateCashUI();

            // Reset the achievement to prevent claiming again
            PlayerPrefs.SetInt(goalKey, 2);  // Using '2' to indicate "claimed" state
            PlayerPrefs.Save();
        }
    }

    public void UpdateCashUI() {
        if (cashBalanceText != null) {
            cashBalanceText.text = $"{PlayerPrefs.GetInt(CashKey, 0)}";
        }

    }

    private void UpdateUI() {
        if (treatsProgressText != null)
            treatsProgressText.text = $"{treatsGiven}";

        if (MiniGameProgressText != null)
            MiniGameProgressText.text = $"{miniGamePlayed}";

        if (TrickProgressText != null)
            TrickProgressText.text = $"{TrickGiven}";

        if (BathProgressText != null)
            BathProgressText.text = $"{BathGiven}";

    }


    int goalsCompletedCount;

    private void IncrementGoalsCompleted() {
        // Increment the slider value
        goalsCompleted.value += 0.20f;
        PlayerPrefs.SetFloat("GoalsSliderValue3", goalsCompleted.value);

        // Increment the text value
        goalsCompletedCount = PlayerPrefs.GetInt("GoalsCompletedCount3", 0);
        goalsCompletedCount++;


        PlayerPrefs.SetInt("GoalsCompletedCount3", goalsCompletedCount);
        goalsCompletedText.text = goalsCompletedCount.ToString();

        PlayerPrefs.Save();

    }


    public void btnGiftClick() {
        Debug.Log("CONGRATS UNLOCK 4 CATS");

    }
}
