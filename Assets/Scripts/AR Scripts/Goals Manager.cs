using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GoalsManager : MonoBehaviour
{
    public static GoalsManager Instance { get; private set; }

    public const string CashKey = "PlayerCash";

    // Define PlayerPrefs keys for goal progress and achievements
    private const string TreatsGoalKey = "TreatsGoalProgress";
    private const string WaterGoalKey = "WaterGoalProgress";
    private const string JumpGoalKey = "JumpGoalProgress";
    private const string PlayTimeGoalKey = "PlayTimeGoalProgress";
    private const string BannerGoalKey = "BannerGoalProgress";

    public const string TreatsGoalAchievedKey = "TreatsGoalAchieved";
    public const string WaterGoalAchievedKey = "WaterGoalAchieved";
    public const string JumpGoalAchievedKey = "JumpGoalAchieved";
    public const string PlayTimeGoalAchievedKey = "PlayTimeGoalAchieved";
    public const string BannerGoalAchievedKey = "BannerGoalAchieved";

    // Cash rewards for each goal
    private const int TreatsGoalCashReward = 20;
    private const int WaterGoalCashReward = 100;
    private const int JumpGoalCashReward = 40;
    private const int PlayTimeGoalCashReward = 20;

    // Goal limits
    private const int TreatsGoalLimit = 5;
    private const int WaterGoalLimit = 5;
    private const int JumpGoalLimit = 20;
    private const int PlayTimeGoalLimit = 5;

    // Goal limits for banner
    private const int BannerGoalLimit = 5;

    private int treatsGiven = 0;
    private int waterGiven = 0;
    private int jumpsPerformed = 0;                                                             
    private int playHours = 0;

    private float playTimeCounter = 0f;

    public TMP_Text treatsProgressText;
    public TMP_Text waterProgressText;
    public TMP_Text jumpProgressText;
    public TMP_Text playTimeProgressText;
    public TMP_Text cashBalanceText;  // Text UI for displaying cash balance
    public TMP_Text goalBannerText; // Banner

    public Image treatsAchievementImage;
    public Image waterAchievementImage;
    public Image jumpAchievementImage;
    public Image playTimeAchievementImage;
    public Sprite achievedSprite;

    public Slider goalsCompleted; // The slider representing goal completion
    public TMP_Text goalsCompletedText; // The text showing the number of completed goals


    private void Start() {
        LoadGoalsProgress();
        LoadAchievementsStatus();
        UpdateUI();
        UpdateCashUI(); // Update cash display on startup

        // Load the slider and text progress
        goalsCompleted.value = PlayerPrefs.GetFloat("GoalsSliderValue", 0f);
        goalsCompletedText.text = PlayerPrefs.GetInt("GoalsCompletedCount", 0).ToString();
    }


    private void Update()
    {
        playTimeCounter += Time.deltaTime;
        if (playTimeCounter >= 3600f)
        {
            IncrementPlayTimeGoal();
            playTimeCounter = 0f;
        }
    }

    public void IncrementTreatsGoal()
    {
        if (treatsGiven < TreatsGoalLimit)
        {
            treatsGiven++;
            SaveGoalProgress(TreatsGoalKey, treatsGiven);
            CheckGoalCompletion();
            UpdateUI();


        }
    }

    public void IncrementWaterGoal()
    {
        if (waterGiven < WaterGoalLimit)
        {
            waterGiven++;
            SaveGoalProgress(WaterGoalKey, waterGiven);
            CheckGoalCompletion();
            UpdateUI();
        }
    }

    public void IncrementJumpGoal()
    {
        if (jumpsPerformed < JumpGoalLimit)
        {
            jumpsPerformed++;
            SaveGoalProgress(JumpGoalKey, jumpsPerformed);
            CheckGoalCompletion();
            UpdateUI();
        }
    }

    private void IncrementPlayTimeGoal()
    {
        if (playHours < PlayTimeGoalLimit)
        {
            playHours++;
            SaveGoalProgress(PlayTimeGoalKey, playHours);
            CheckGoalCompletion();
            UpdateUI();
        }
    }

    private void SaveGoalProgress(string key, int progress)
    {
        PlayerPrefs.SetInt(key, progress);
        PlayerPrefs.Save();
    }

    private void LoadGoalsProgress()
    {
        treatsGiven = PlayerPrefs.GetInt(TreatsGoalKey, 0);
        waterGiven = PlayerPrefs.GetInt(WaterGoalKey, 0);
        jumpsPerformed = PlayerPrefs.GetInt(JumpGoalKey, 0);
        playHours = PlayerPrefs.GetInt(PlayTimeGoalKey, 0);
    }

    private void LoadAchievementsStatus()
    {
        if ((PlayerPrefs.GetInt(TreatsGoalAchievedKey, 0) == 1 || PlayerPrefs.GetInt(TreatsGoalAchievedKey, 0) == 2) && achievedSprite != null)
            treatsAchievementImage.sprite = achievedSprite;

        if ((PlayerPrefs.GetInt(WaterGoalAchievedKey, 0) == 1 || PlayerPrefs.GetInt(WaterGoalAchievedKey, 0) == 2) && achievedSprite != null)
            waterAchievementImage.sprite = achievedSprite;

        if ((PlayerPrefs.GetInt(JumpGoalAchievedKey, 0) == 1 || PlayerPrefs.GetInt(JumpGoalAchievedKey, 0) == 2) && achievedSprite != null)
            jumpAchievementImage.sprite = achievedSprite;

        if ((PlayerPrefs.GetInt(PlayTimeGoalAchievedKey, 0) == 1 || PlayerPrefs.GetInt(PlayTimeGoalAchievedKey, 0) == 2) && achievedSprite != null)
            playTimeAchievementImage.sprite = achievedSprite;
    }


    private void CheckGoalCompletion() {
        bool goalAchieved = false;

        if (treatsGiven >= TreatsGoalLimit && PlayerPrefs.GetInt(TreatsGoalAchievedKey, 0) == 0) {
            treatsAchievementImage.sprite = achievedSprite;
            PlayerPrefs.SetInt(TreatsGoalAchievedKey, 1);
            goalAchieved = true;
        }

        if (waterGiven >= WaterGoalLimit && PlayerPrefs.GetInt(WaterGoalAchievedKey, 0) == 0) {
            waterAchievementImage.sprite = achievedSprite;
            PlayerPrefs.SetInt(WaterGoalAchievedKey, 1);
            goalAchieved = true;
        }

        if (jumpsPerformed >= JumpGoalLimit && PlayerPrefs.GetInt(JumpGoalAchievedKey, 0) == 0) {
            jumpAchievementImage.sprite = achievedSprite;
            PlayerPrefs.SetInt(JumpGoalAchievedKey, 1);
            goalAchieved = true;
        }

        if (playHours >= PlayTimeGoalLimit && PlayerPrefs.GetInt(PlayTimeGoalAchievedKey, 0) == 0) {
            playTimeAchievementImage.sprite = achievedSprite;
            PlayerPrefs.SetInt(PlayTimeGoalAchievedKey, 1);
            goalAchieved = true;
        }

        // Update the slider and text if a goal was achieved
        if (goalAchieved) {
            IncrementGoalsCompleted();
        }

        PlayerPrefs.Save();
    }


    public void ClaimReward(string goalKey)
    {
        int cashReward = 0;
                        
        switch (goalKey)
        {
            case TreatsGoalAchievedKey:
                if (PlayerPrefs.GetInt(TreatsGoalAchievedKey, 0) == 1)
                    cashReward = TreatsGoalCashReward;
                break;

            case WaterGoalAchievedKey:
                if (PlayerPrefs.GetInt(WaterGoalAchievedKey, 0) == 1)
                    cashReward = WaterGoalCashReward;
                break;

            case JumpGoalAchievedKey:
                if (PlayerPrefs.GetInt(JumpGoalAchievedKey, 0) == 1)
                    cashReward = JumpGoalCashReward;
                break;

            case PlayTimeGoalAchievedKey:
                if (PlayerPrefs.GetInt(PlayTimeGoalAchievedKey, 0) == 1)
                    cashReward = PlayTimeGoalCashReward;
                break;
        }

        if (cashReward > 0)
        {
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

    private void UpdateCashUI()
    {

        cashBalanceText.text = "500";
        //if (cashBalanceText != null)
        //    cashBalanceText.text = $"{PlayerPrefs.GetInt(CashKey, 0)}";
    }

    private void UpdateUI()
    {
        if (treatsProgressText != null)
            treatsProgressText.text = $"{treatsGiven}";

        if (waterProgressText != null)
            waterProgressText.text = $"{waterGiven}";

        if (jumpProgressText != null)
            jumpProgressText.text = $"{jumpsPerformed}";

        if (playTimeProgressText != null)
            playTimeProgressText.text = $"{playHours}";
    }

    private void IncrementGoalsCompleted()
{
    // Increment the slider value
    goalsCompleted.value += 0.20f;
    PlayerPrefs.SetFloat("GoalsSliderValue", goalsCompleted.value);

    // Increment the text value
    int goalsCompletedCount = PlayerPrefs.GetInt("GoalsCompletedCount", 0);
    goalsCompletedCount++;
    PlayerPrefs.SetInt("GoalsCompletedCount", goalsCompletedCount);
    goalsCompletedText.text = goalsCompletedCount.ToString();

    PlayerPrefs.Save();
}

}
