using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CatStatus : MonoBehaviour
{
    [SerializeField]
    private Transform[] heartSpawnPoints; // Assign spawn points in the Inspector

    public GameObject hungerEmote;
    public GameObject affectionEmote;
    public GameObject thirstEmote;
    public GameObject sickEmote;
    public GameObject dirtyEmote;

    public float hungerLevel = 0f;
    public float affectionLevel = 0f;
    public float thirstLevel = 0f;

    public float decreaseRateAffection = 0.0023f; // Decrease per second
    public float decreaseRateHunger = 0.0023f;    // Decrease per second
    public float decreaseRateThirst = 0.0046f;    // Decrease per second

    private string catID;
    public bool isSick;
    public bool isDirty;

    private Vector3 initialPositionHunger;
    private Vector3 initialPositionAffection;
    private Vector3 initialPositionThirst;

    public GameObject loadingScreen; // Assign the loading screen GameObject in the Inspector
    public Slider loadingSlider;     // Assign the Slider UI element in the Inspector

    public bool isBeingPetted = false;

    FakeLoadingScreen fakeLoadingScreen;

    private float emoteOffset = 0.001f; // The offset to use for separating emotes

    //GoalsManager goalsManager;
    [SerializeField] private GoalsManager goalsManager;
    [SerializeField] private GoalsManagerTier2 goalsManagerTier2;
    [SerializeField] private GoalsManagerTier3 goalsManagerTier3;
    private const string GoalsCounterKey = "GoalsCounter";

    void Start()
    {
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


        // Initialize other components
        catID = gameObject.name;
        initialPositionHunger = hungerEmote.transform.localPosition;
        initialPositionAffection = affectionEmote.transform.localPosition;
        initialPositionThirst = thirstEmote.transform.localPosition;
        fakeLoadingScreen = GameObject.Find("Canvas").GetComponent<FakeLoadingScreen>();

        LoadCatStatus();
        StartCoroutine(UpdateStatus());
    }


    private IEnumerator UpdateStatus()
    {
        while (true)
        {
            // Calculate time since the last update
            float timeSinceLastUpdate = Time.deltaTime;

            // Update status based on elapsed time
            hungerLevel -= timeSinceLastUpdate * decreaseRateHunger;
            affectionLevel -= timeSinceLastUpdate * decreaseRateAffection;
            thirstLevel -= timeSinceLastUpdate * decreaseRateThirst;

            hungerLevel = Mathf.Clamp(hungerLevel, 0f, 100f);
            affectionLevel = Mathf.Clamp(affectionLevel, 0f, 100f);
            thirstLevel = Mathf.Clamp(thirstLevel, 0f, 100f);

            CheckForSickness(); // Check if the cat should become sick
            CheckForDirtiness();

            // Update emotes and save the status
            UpdateEmotes();
            SaveCatStatus();

            yield return null;
        }
    }

    private void CheckForSickness()
    {
        if (!PlayerPrefs.HasKey(catID + "_LastFed"))
        {
            PlayerPrefs.SetString(catID + "_LastFed", DateTime.Now.ToString());
            return;
        }

        DateTime lastFedTime = DateTime.Parse(PlayerPrefs.GetString(catID + "_LastFed"));
        TimeSpan timeSinceLastFed = DateTime.Now - lastFedTime;

        // If more than a day has passed since the cat was last fed, it gets sick
        if (timeSinceLastFed.TotalMilliseconds >= 100 && !isSick)
        {
            isSick = true;
            Debug.Log($"{catID} has gotten sick due to not being fed for a day.");
        }
    }

    private void CheckForDirtiness()
    {
        if (!PlayerPrefs.HasKey(catID + "_LastPetted"))
        {
            PlayerPrefs.SetString(catID + "_LastPetted", DateTime.Now.ToString());
            return;
        }

        DateTime lastPettedTime = DateTime.Parse(PlayerPrefs.GetString(catID + "_LastPetted"));
        TimeSpan timeSinceLastPetted = DateTime.Now - lastPettedTime;

        // If more than a day has passed since the cat was last petted, it gets dirty
        if (timeSinceLastPetted.TotalMilliseconds >= 100 && !isDirty)
        {
            isDirty = true;
            
            Debug.Log($"{catID} has gotten dirty due to not being petted for a day.");
        }
    }

    private void UpdateEmotes()
    {
        // Create a list to hold active emotes
        List<GameObject> activeEmotes = new List<GameObject>();

        // Check which emotes should be active and add them to the list
        if (hungerLevel <= 30f) activeEmotes.Add(hungerEmote);
        if (affectionLevel <= 30f) activeEmotes.Add(affectionEmote);
        if (thirstLevel <= 30f) activeEmotes.Add(thirstEmote);
        if (isSick) activeEmotes.Add(sickEmote);
        if (isDirty) activeEmotes.Add(dirtyEmote);

        // Deactivate all emotes initially
        hungerEmote.SetActive(false);
        affectionEmote.SetActive(false);
        thirstEmote.SetActive(false);
        sickEmote.SetActive(false);
        dirtyEmote.SetActive(false);    

        // Calculate the spacing for active emotes
        int activeEmoteCount = activeEmotes.Count;
        if (activeEmoteCount > 0)
        {
            float totalWidth = (activeEmoteCount - 1) * emoteOffset; // Total width for emotes
            float startingX = -(totalWidth / 2f); // Center the emotes

            for (int i = 0; i < activeEmoteCount; i++)
            {
                GameObject emote = activeEmotes[i];
                emote.SetActive(true);
                emote.transform.localPosition = initialPositionHunger + new Vector3(startingX + (emoteOffset * i), 0, 0);
            }
        }
    }


    private void SaveCatStatus()
    {
        PlayerPrefs.SetFloat(catID + "_Hunger", hungerLevel);
        PlayerPrefs.SetFloat(catID + "_Affection", affectionLevel);
        PlayerPrefs.SetFloat(catID + "_Thirst", thirstLevel);

        PlayerPrefs.SetInt(catID + "_IsSick", isSick ? 1 : 0);
        PlayerPrefs.SetInt(catID + "_IsDirty", isDirty ? 1 : 0);
        PlayerPrefs.SetString(catID + "_LastUpdate", DateTime.Now.ToString());

        PlayerPrefs.Save();
    }

    private void LoadCatStatus()
    {
        hungerLevel = PlayerPrefs.GetFloat(catID + "_Hunger", 100f);
        affectionLevel = PlayerPrefs.GetFloat(catID + "_Affection", 100f);
        thirstLevel = PlayerPrefs.GetFloat(catID + "_Thirst", 100f);

        isSick = PlayerPrefs.GetInt(catID + "_IsSick", 0) == 1;
        isDirty = PlayerPrefs.GetInt(catID + "_IsDirty", 0) == 1;

        if (PlayerPrefs.HasKey(catID + "_LastUpdate"))
        {
            DateTime lastUpdate = DateTime.Parse(PlayerPrefs.GetString(catID + "_LastUpdate"));
            float timeSinceLastUpdate = (float)(DateTime.Now - lastUpdate).TotalSeconds;

            hungerLevel -= timeSinceLastUpdate * decreaseRateHunger;
            affectionLevel -= timeSinceLastUpdate * decreaseRateAffection;
            thirstLevel -= timeSinceLastUpdate * decreaseRateThirst;

            hungerLevel = Mathf.Clamp(hungerLevel, 0f, 100f);
            affectionLevel = Mathf.Clamp(affectionLevel, 0f, 100f);
            thirstLevel = Mathf.Clamp(thirstLevel, 0f, 100f);
        }
    }

    public void FeedCat()
    {
        hungerLevel += 100f;
        hungerLevel = Mathf.Clamp(hungerLevel, 0f, 100f);

        // Update the last fed time
        PlayerPrefs.SetString(catID + "_LastFed", DateTime.Now.ToString());

        SaveCatStatus();
        Debug.Log($"{catID} has been fed. Hunger level is now {hungerLevel}.");
    }

    public void TreatCat()
    {
        hungerLevel += 30f;
        hungerLevel = Mathf.Clamp(hungerLevel, 0f, 100f);

        PlayerPrefs.SetString(catID + "_LastFed", DateTime.Now.ToString());
        SaveCatStatus();
        Debug.Log($"{catID} has been treated. Hunger level is now {hungerLevel}.");
    }

    public void StartPettingAffection()
    {
        isBeingPetted = true;
        StartCoroutine(IncreaseAffectionGradually());
    }

    public void StopPettingAffection()
    {
        isBeingPetted = false;
        StopCoroutine(IncreaseAffectionGradually());
    }

    private IEnumerator IncreaseAffectionGradually()
    {
        while (isBeingPetted)
        {
            PlayerPrefs.SetString(catID + "_LastPetted", DateTime.Now.ToString());
            affectionLevel += 50f; // Increase affection gradually
            SaveCatStatus(); // Save the status after each increase
            yield return new WaitForSeconds(0.5f); // Wait for 0.5 seconds before increasing again
        }
    }

    public void GiveWater()
    {
        thirstLevel = 100f;
        SaveCatStatus();
    }

    public Transform[] GetHeartSpawnPoints()
    {
        if (heartSpawnPoints != null && heartSpawnPoints.Length > 0)
        {
            return heartSpawnPoints;
        }

        Debug.LogWarning("Heart spawn points not assigned for " + gameObject.name);
        return null;
    }

    public void RemoveSickStatus()
    {
        if (isSick)
        {
            isSick = false;
            SaveCatStatus();
            Debug.Log($"{catID}: Sick status removed.");
        }
        else
        {
            Debug.Log($"{catID}: Cat is already clean and healthy.");
        }
    }

    public void RemoveDirtyStatus()
    {
        if (isDirty) {
            isDirty = false;
            SaveCatStatus();
            Debug.Log($"{catID}: Dirty status removed.");

            // Goals manager
            if (PlayerPrefs.GetInt(GoalsCounterKey, 0) == 1) {
                goalsManager.IncrementBathGoal();  // Tier 1
            }

            if (PlayerPrefs.GetInt(GoalsCounterKey, 0) == 2) {
                goalsManagerTier2.IncrementBathGoal(); //Tier2
            }

            if (PlayerPrefs.GetInt(GoalsCounterKey, 0) == 3) {

                goalsManagerTier3.IncrementBathGoal(); //Tier3
            }


        }
        else
        {
            Debug.Log($"{catID}: Cat is already clean and healthy.");
        }
    }

    
}
