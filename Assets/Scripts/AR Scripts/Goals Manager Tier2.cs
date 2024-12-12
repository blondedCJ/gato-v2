using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GoalsManagerTier2 : MonoBehaviour
{

    public static GoalsManagerTier2 Instance { get; private set; }

    public const string CashKey = "PlayerCash";

    // Define PlayerPrefs keys for goal progress and achievements
    private const string TreatsGoalKey = "TreatsGoalProgress2";
    public const string TreatsGoalAchievedKey = "TreatsGoalAchieved2";

    // Cash rewards for each goal
    private const int TreatsGoalCashReward = 30;

    // Goal limits
    private const int TreatsGoalLimit = 10;

    // Goal limits for banner
    private const int BannerGoalLimit = 5;

    private int treatsGiven = 0;

    public TMP_Text treatsProgressText;

    public TMP_Text cashBalanceText;  // Text UI for displaying cash balance

    public Image treatsAchievementImage;

    public Sprite achievedSprite;

    public Slider goalsCompleted; // The slider representing goal completion
    public TMP_Text goalsCompletedText; // The text showing the number of completed goals

    public GameObject NameYourCat;
    public GameObject PickACat;

    public Image treatsDescriptionImage;
    public Image miniGameDescriptionImage;
    public Image tricksDescriptionImage;
    public Image clinicDescriptionImage;
    public Image bathDescriptionImage;
    public Sprite achievedHighlight;

    public GameObject redNotification; // Assign this in the Unity Inspector

    // Unlock Cosmetics
    [SerializeField] private CosmeticManager cosmeticManager;

    private void Start() {


        // Dynamically find Cosmetics manager if not assigned via Inspector
        if (cosmeticManager == null) {
            cosmeticManager = FindObjectOfType<CosmeticManager>();
            if (cosmeticManager == null) {
                Debug.LogError("Cosmetics manager is not assigned or found!");
            }
        }

        UpdateRedNotification();  // Initialize the red notification state
        LoadHighlightState();  // Load and apply the highlight state
        checkPanel1();
        LoadGoalsProgress();
        LoadAchievementsStatus();
        UpdateUI();
        UpdateCashUI(); // Update cash display on startup

        // Load the slider and text progress
        goalsCompleted.value = PlayerPrefs.GetFloat("GoalsSliderValue2", 0f);
        goalsCompletedText.text = PlayerPrefs.GetInt("GoalsCompletedCount2", 0).ToString();
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
    private const string MiniGameGoalKey = "MiniGameGoalProgress2";
    public const string MiniGameAchievedKey = "MiniGameGoalAchieved2";
    private const int MiniGameGoalCashReward = 150;
    public TMP_Text MiniGameProgressText;
    public Image MiniGameAchievementImage;
    int miniGamePlayed;
    int miniGamePlayedLimit = 20;
    public void IncrementPlayMiniGameGoal() {
        if (miniGamePlayed < miniGamePlayedLimit) {
            miniGamePlayed++;
            SaveGoalProgress(MiniGameGoalKey, miniGamePlayed);
            CheckGoalCompletion();
            UpdateUI();
        }
    }

    // Perform trick
    private const string TrickGoalKey = "TrickGoalProgress2";
    public const string TrickAchievedKey = "TrickGoalAchieved2";
    private const int TrickGoalCashReward = 150;
    public TMP_Text TrickProgressText;
    public Image TrickAchievementImage;
    int TrickGiven;
    int TrickLimit = 15;
    public void IncrementPerformTrickGoal() {
        if (TrickGiven < TrickLimit) {
            TrickGiven++;
            SaveGoalProgress(TrickGoalKey, TrickGiven);
            CheckGoalCompletion();
            UpdateUI();
        }

    }

    // Bathing
    private const string BathGoalKey = "BathGoalProgress2";
    public const string BathAchievedKey = "BathGoalAchieved2";
    private const int BathGoalCashReward = 65;
    public TMP_Text BathProgressText;
    public Image BathAchievementImage;
    int BathGiven;
    int BathLimit = 8;
    public void IncrementBathGoal() {

        if (BathGiven < BathLimit) {
            BathGiven++;
            SaveGoalProgress(BathGoalKey, BathGiven);
            CheckGoalCompletion();
            UpdateUI();
        }

    }


    // Clinic
    private const string ClinicGoalKey = "ClinicGoalProgres2";
    public const string ClinicAchievedKey = "ClinicGoalAchieved2";
    private const int ClinicGoalCashReward = 65;
    public TMP_Text ClinicProgressText;
    public Image ClinicAchievementImage;
    int ClinicGiven;
    int ClinicLimit = 10;
    public void IncrementClinicGoal() {
        if (ClinicGiven < ClinicLimit) {
            ClinicGiven++;
            SaveGoalProgress(ClinicGoalKey, ClinicGiven);
            CheckGoalCompletion();
            UpdateUI();
        }

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
        ClinicGiven = PlayerPrefs.GetInt(ClinicGoalKey, 0);
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

        //clinic
        if ((PlayerPrefs.GetInt(ClinicAchievedKey, 0) == 1
            || PlayerPrefs.GetInt(ClinicAchievedKey, 0) == 2)
            && achievedSprite != null)
            ClinicAchievementImage.sprite = achievedSprite;
    }


    private void CheckGoalCompletion() {
        bool goalAchieved = false;

        //treats
        if (treatsGiven >= TreatsGoalLimit && PlayerPrefs.GetInt(TreatsGoalAchievedKey, 0) == 0) {
            treatsAchievementImage.sprite = achievedSprite;
            PlayerPrefs.SetInt(TreatsGoalAchievedKey, 1);
            goalAchieved = true;
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayAchievementUnlocked(); // Play the click sound
            }
        }

        //minigame
        if (miniGamePlayed >= miniGamePlayedLimit && PlayerPrefs.GetInt(MiniGameAchievedKey, 0) == 0) {
            MiniGameAchievementImage.sprite = achievedSprite;
            PlayerPrefs.SetInt(MiniGameAchievedKey, 1);
            goalAchieved = true;
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayAchievementUnlocked(); // Play the click sound
            }
        }

        // perform tricks
        if (TrickGiven >= TrickLimit && PlayerPrefs.GetInt(TrickAchievedKey, 0) == 0) {
            TrickAchievementImage.sprite = achievedSprite;
            PlayerPrefs.SetInt(TrickAchievedKey, 1);
            goalAchieved = true;
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayAchievementUnlocked(); // Play the click sound
            }
        }

        // bathing 
        if (BathGiven >= BathLimit && PlayerPrefs.GetInt(BathAchievedKey, 0) == 0) {
            BathAchievementImage.sprite = achievedSprite;
            PlayerPrefs.SetInt(BathAchievedKey, 1);
            goalAchieved = true;
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayAchievementUnlocked(); // Play the click sound
            }
        }

        // clinic 
        if (ClinicGiven >= ClinicLimit && PlayerPrefs.GetInt(ClinicAchievedKey, 0) == 0) {
            ClinicAchievementImage.sprite = achievedSprite;
            PlayerPrefs.SetInt(ClinicAchievedKey, 1);
            goalAchieved = true;
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayAchievementUnlocked(); // Play the click sound
            }
        }



        // Update the slider and text if a goal was achieved
        if (goalAchieved) {
            IncrementGoalsCompleted();
            UpdateRedNotification();  // Update the red notification status
        }

        PlayerPrefs.Save();
    }


    public void ClaimReward(string goalKey) {
        int cashReward = 0;


        // Check if the reward has already been claimed (state 2 means claimed)
        if (PlayerPrefs.GetInt(goalKey, 0) == 2) {
            Debug.Log("Reward for " + goalKey + " has already been claimed.");
            return; // Exit early if already claimed
        }

        switch (goalKey) {
            case TreatsGoalAchievedKey:
                if (PlayerPrefs.GetInt(TreatsGoalAchievedKey, 0) == 1) {
                    cashReward = TreatsGoalCashReward;
                    //Unlock one item
                    cosmeticManager.ClaimRandomCosmetic();
                    // Highlight and save the state
                    treatsDescriptionImage.sprite = achievedHighlight;
                    PlayerPrefs.SetInt("TreatsGoalHighlighted2", 1);  // Save highlight state (1 for highlighted)
                }
                break;

            case MiniGameAchievedKey:
                if (PlayerPrefs.GetInt(MiniGameAchievedKey, 0) == 1) {
                    cashReward = MiniGameGoalCashReward;
                    //Unlock one item
                    cosmeticManager.ClaimRandomCosmetic();
                    // Highlight and save the state
                    miniGameDescriptionImage.sprite = achievedHighlight;
                    PlayerPrefs.SetInt("MiniGameGoalHighlighted2", 1);  // Save highlight state (1 for highlighted)

                }
                break;

            case TrickAchievedKey:
                if (PlayerPrefs.GetInt(TrickAchievedKey, 0) == 1) {
                    cashReward = TrickGoalCashReward;
                    //Unlock one item
                    cosmeticManager.ClaimRandomCosmetic();
                    // Highlight and save the state
                    tricksDescriptionImage.sprite = achievedHighlight;
                    PlayerPrefs.SetInt("TricksGoalHighlighted2", 1);  // Save highlight state (1 for highlighted)

                }
                break;

            case BathAchievedKey:
                if (PlayerPrefs.GetInt(BathAchievedKey, 0) == 1) {
                    cashReward = BathGoalCashReward;
                    //Unlock one item
                    cosmeticManager.ClaimRandomCosmetic();
                    // Highlight and save the state
                    bathDescriptionImage.sprite = achievedHighlight;
                    PlayerPrefs.SetInt("BathGoalHighlighted2", 1);  // Save highlight state (1 for highlighted)

                }
                break;

            case ClinicAchievedKey:
                if (PlayerPrefs.GetInt(ClinicAchievedKey, 0) == 1) {
                    cashReward = ClinicGoalCashReward;
                    //Unlock one item
                    cosmeticManager.ClaimRandomCosmetic();
                    // Highlight and save the state
                    clinicDescriptionImage.sprite = achievedHighlight;
                    PlayerPrefs.SetInt("ClinicGoalHighlighted2", 1);  // Save highlight state (1 for highlighted)
                }
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

            UpdateRedNotification();  // Check and update the red notification after claiming a reward
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

        if (ClinicProgressText != null)
            ClinicProgressText.text = $"{ClinicGiven}";

    }


    int goalsCompletedCount;

    private void IncrementGoalsCompleted() {
        // Increment the slider value
        goalsCompleted.value += 0.20f;
        PlayerPrefs.SetFloat("GoalsSliderValue2", goalsCompleted.value);

        // Increment the text value
        goalsCompletedCount = PlayerPrefs.GetInt("GoalsCompletedCount2", 0);
        goalsCompletedCount++;


        PlayerPrefs.SetInt("GoalsCompletedCount2", goalsCompletedCount);
        goalsCompletedText.text = goalsCompletedCount.ToString();

        PlayerPrefs.Save();

    }


    public GameObject panel1;
    private const string Panel1StateKey = "Panel1State2"; // Key to store panel state
    private const string GoalsCounterKey = "GoalsCounter"; // for condition to different scripts


    public void btnGiftClick() {
        if (PlayerPrefs.GetInt("GoalsCompletedCount2", 0) == 5) { // PlayerPrefs.GetInt("GoalsCompletedCount", 0) == 5
            Debug.Log("5.");
            // Set panel1 to inactive
            panel1.SetActive(false);

            NameYourCat.SetActive(true);
            PickACat.SetActive(true);

            redNotification.SetActive(false);

            //save counter 2
            PlayerPrefs.SetInt(GoalsCounterKey, 3);
            PlayerPrefs.Save();
            Debug.Log("Goals Counter Key Value >>>>>>>>>>>>>>>>>>>>>>>>>>>" + PlayerPrefs.GetInt(GoalsCounterKey, 0));

            // Persist the state of panel1 as "inactive" (0)
            PlayerPrefs.SetInt(Panel1StateKey, 0); // 0 means inactive
            PlayerPrefs.Save(); // Save PlayerPrefs
        } else {
            Debug.Log("Not 5.");
        }

    }
    private void checkPanel1() {
        // Retrieve the state of panel1 from PlayerPrefs (default: 1, active)
        int panel1State = PlayerPrefs.GetInt(Panel1StateKey, 1);

        // Set panel1's active state based on the retrieved value
        panel1.SetActive(panel1State == 1); // Active if the state is 1, otherwise inactive
    }

    private void LoadHighlightState() {
        // Load the saved highlight state for each goal and apply the highlight if needed
        if (PlayerPrefs.GetInt("TreatsGoalHighlighted2", 0) == 1) {
            treatsDescriptionImage.sprite = achievedHighlight;
        }

        if (PlayerPrefs.GetInt("MiniGameGoalHighlighted2", 0) == 1) {
            miniGameDescriptionImage.sprite = achievedHighlight;
        }

        if (PlayerPrefs.GetInt("TrickGoalHighlighted2", 0) == 1) {
            tricksDescriptionImage.sprite = achievedHighlight;
        }

        if (PlayerPrefs.GetInt("BathGoalHighlighted2", 0) == 1) {
            bathDescriptionImage.sprite = achievedHighlight;
        }

        if (PlayerPrefs.GetInt("ClinicGoalHighlighted2", 0) == 1) {
            clinicDescriptionImage.sprite = achievedHighlight;
        }
    }


    private void UpdateRedNotification() {
        int claimableRewards = 0;

        // Check each goal's achievement state (1 = achieved but not claimed)
        if (PlayerPrefs.GetInt(TreatsGoalAchievedKey, 0) == 1) claimableRewards++;
        if (PlayerPrefs.GetInt(MiniGameAchievedKey, 0) == 1) claimableRewards++;
        if (PlayerPrefs.GetInt(TrickAchievedKey, 0) == 1) claimableRewards++;
        if (PlayerPrefs.GetInt(BathAchievedKey, 0) == 1) claimableRewards++;
        if (PlayerPrefs.GetInt(ClinicAchievedKey, 0) == 1) claimableRewards++;

        // Set the red notification active if there are unclaimed rewards
        if (redNotification != null) {
            redNotification.SetActive(claimableRewards > 0);
        }
    }


}
