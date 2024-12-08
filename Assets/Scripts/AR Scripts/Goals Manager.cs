using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class GoalsManager : MonoBehaviour
{
    public static GoalsManager Instance { get; private set; }

    public const string CashKey = "PlayerCash";

    // Define PlayerPrefs keys for goal progress and achievements
    private const string TreatsGoalKey = "TreatsGoalProgress";
    public const string TreatsGoalAchievedKey = "TreatsGoalAchieved";

    // Cash rewards for each goal
    private const int TreatsGoalCashReward = 20;

    // Goal limits
    private const int TreatsGoalLimit = 5;

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

        // Check if the GoalsCounterKey is already set
        if (!PlayerPrefs.HasKey(GoalsCounterKey)) {
            // If the key doesn't exist, set the value to 1
            PlayerPrefs.SetInt(GoalsCounterKey, 1);
            PlayerPrefs.Save(); // Ensure the changes are saved
            Debug.Log("GoalsCounterKey set to 1");
        } else {
            // If the key already exists, do nothing or perform some other logic
            Debug.Log("GoalsCounterKey already exists, value: " + PlayerPrefs.GetInt(GoalsCounterKey));
        }


        checkPanel1();
        LoadGoalsProgress();
        LoadAchievementsStatus();
        UpdateUI();
        UpdateCashUI(); // Update cash display on startup

        // Load the slider and text progress
        goalsCompleted.value = PlayerPrefs.GetFloat("GoalsSliderValue", 0f);
        goalsCompletedText.text = PlayerPrefs.GetInt("GoalsCompletedCount", 0).ToString();
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
    private const string MiniGameGoalKey = "MiniGameGoalProgress";
    public const string MiniGameAchievedKey = "MiniGameGoalAchieved";
    private const int MiniGameGoalCashReward = 100;
    public TMP_Text MiniGameProgressText;
    public Image MiniGameAchievementImage;
    int miniGamePlayed;
    int miniGamePlayedLimit = 10;
    public void IncrementPlayMiniGameGoal() {
        if (miniGamePlayed < miniGamePlayedLimit) {
            miniGamePlayed++;
            SaveGoalProgress(MiniGameGoalKey, miniGamePlayed);
            CheckGoalCompletion();
            UpdateUI();
        }
    }

    // Perform trick
    private const string TrickGoalKey = "TrickGoalProgress";
    public const string TrickAchievedKey = "TrickGoalAchieved";
    private const int TrickGoalCashReward = 100;
    public TMP_Text TrickProgressText;
    public Image TrickAchievementImage;
    int TrickGiven;
    int TrickLimit = 5;
    public void IncrementPerformTrickGoal() {
        if (TrickGiven < TrickLimit) {
            TrickGiven++;
            SaveGoalProgress(TrickGoalKey, TrickGiven);
            CheckGoalCompletion();
            UpdateUI();
        }

    }

    // Bathing
    private const string BathGoalKey = "BathGoalProgress";
    public const string BathAchievedKey = "BathGoalAchieved";
    private const int BathGoalCashReward = 50;
    public TMP_Text BathProgressText;
    public Image BathAchievementImage;
    int BathGiven;
    int BathLimit = 2;
    public void IncrementBathGoal() {
        
        if (BathGiven < BathLimit) {
            BathGiven++;
            SaveGoalProgress(BathGoalKey, BathGiven);
            CheckGoalCompletion();
            UpdateUI();
        }

    }


    // Clinic
    private const string ClinicGoalKey = "ClinicGoalProgress";
    public const string ClinicAchievedKey = "ClinicGoalAchieved";
    private const int ClinicGoalCashReward = 40;
    public TMP_Text ClinicProgressText;
    public Image ClinicAchievementImage;
    int ClinicGiven;
    int ClinicLimit = 5;
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

        // clinic 
        if (ClinicGiven >= ClinicLimit && PlayerPrefs.GetInt(ClinicAchievedKey, 0) == 0) {
            ClinicAchievementImage.sprite = achievedSprite;
            PlayerPrefs.SetInt(ClinicAchievedKey, 1);
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

        // Check if the reward has already been claimed (state 2 means claimed)
        if (PlayerPrefs.GetInt(goalKey, 0) == 2 ) {
            Debug.Log("Reward for " + goalKey + " has already been claimed.");
            return; // Exit early if already claimed
        }

        switch (goalKey) {
            case TreatsGoalAchievedKey:
                if (PlayerPrefs.GetInt(TreatsGoalAchievedKey, 0) == 1) {
                    cashReward = TreatsGoalCashReward;
                    //Unlock one item
                    cosmeticManager.ClaimRandomCosmetic();
                }
                    
                break;

            case MiniGameAchievedKey:
                if (PlayerPrefs.GetInt(MiniGameAchievedKey, 0) == 1) {
                    cashReward = MiniGameGoalCashReward;
                    //Unlock one item
                    cosmeticManager.ClaimRandomCosmetic();
                }
                break;

            case TrickAchievedKey:
                if (PlayerPrefs.GetInt(TrickAchievedKey, 0) == 1) {
                    cashReward = TrickGoalCashReward;
                    //Unlock one item
                    cosmeticManager.ClaimRandomCosmetic();

                }
                break;

            case BathAchievedKey:
                if (PlayerPrefs.GetInt(BathAchievedKey, 0) == 1) {
                    cashReward = BathGoalCashReward;
                    //Unlock one item
                    cosmeticManager.ClaimRandomCosmetic();

                }
                break;

            case ClinicAchievedKey:
                if (PlayerPrefs.GetInt(ClinicAchievedKey, 0) == 1) {
                    cashReward = ClinicGoalCashReward;
                    //Unlock one item
                    cosmeticManager.ClaimRandomCosmetic();

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
        PlayerPrefs.SetFloat("GoalsSliderValue", goalsCompleted.value);

        // Increment the text value
        goalsCompletedCount = PlayerPrefs.GetInt("GoalsCompletedCount", 0);
        goalsCompletedCount++;


        PlayerPrefs.SetInt("GoalsCompletedCount", goalsCompletedCount);
        goalsCompletedText.text = goalsCompletedCount.ToString();

        PlayerPrefs.Save();

    }


    public GameObject panel1;
    private const string Panel1StateKey = "Panel1State"; // Key to store panel state
    private const string GoalsCounterKey = "GoalsCounter"; // for condition to different scripts


    public void btnGiftClick() {
        if (PlayerPrefs.GetInt("GoalsCompletedCount", 0) == 5) { // PlayerPrefs.GetInt("GoalsCompletedCount", 0) == 5
            Debug.Log("5.");
            // Set panel1 to inactive
            panel1.SetActive(false);

            NameYourCat.SetActive(true);
            PickACat.SetActive(true);

            //save counter 2
            PlayerPrefs.SetInt(GoalsCounterKey, 2);
            PlayerPrefs.Save();

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

}
