using System.Collections;
using UnityEngine;

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

    private float timeSinceHungry = 0f;
    private float timeSinceUnpetted = 0f;

    private string catID;
    private Vector3 initialPositionHunger;
    private Vector3 initialPositionAffection;
    private Vector3 initialPositionThirst;

    public bool isBeingPetted = false;

    private float emoteOffset = 0.001f; // The offset to use for separating emotes

    void Start()
    {
        catID = gameObject.name;

        // Save the initial positions of the emotes
        initialPositionHunger = hungerEmote.transform.localPosition;
        initialPositionAffection = affectionEmote.transform.localPosition;
        initialPositionThirst = thirstEmote.transform.localPosition;

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

            // Track time since the cat has been hungry or unpetted
            if (hungerLevel <= 20f) timeSinceHungry += timeSinceLastUpdate;
            else timeSinceHungry = 0f;

            if (!isBeingPetted && affectionLevel <= 20f) timeSinceUnpetted += timeSinceLastUpdate;
            else timeSinceUnpetted = 0f;

            UpdateEmotes();
            SaveCatStatus();

            yield return null;
        }
    }

    private void UpdateEmotes()
    {
        // Count how many emotes will be active
        int activeEmoteCount = 0;

        // Determine how many emotes are active
        if (hungerLevel <= 30f) activeEmoteCount++;
        if (affectionLevel <= 30f) activeEmoteCount++;
        if (thirstLevel <= 30f) activeEmoteCount++;
        if (timeSinceHungry > 30f) activeEmoteCount++; // Sick condition
        if (timeSinceUnpetted > 60f) activeEmoteCount++; // Dirty condition

        // Base position (centered)
        Vector3 basePosition = initialPositionHunger; // Use hunger emote's initial position as the center
        int currentEmoteIndex = 0; // To track the placement of each active emote

        // Calculate starting offset based on the number of active emotes
        float startingOffset = -(activeEmoteCount - 1) * emoteOffset / 2;

        // Show hunger emote if hunger level is low and position it
        if (hungerLevel <= 30f)
        {
            hungerEmote.SetActive(true);
            hungerEmote.transform.localPosition = basePosition + new Vector3(startingOffset + emoteOffset * currentEmoteIndex, 0, 0);
            currentEmoteIndex++;
        }
        else
        {
            hungerEmote.SetActive(false);
        }

        // Show affection emote if affection level is low and position it
        if (affectionLevel <= 30f)
        {
            affectionEmote.SetActive(true);
            affectionEmote.transform.localPosition = basePosition + new Vector3(startingOffset + emoteOffset * currentEmoteIndex, 0, 0);
            currentEmoteIndex++;
        }
        else
        {
            affectionEmote.SetActive(false);
        }

        // Show thirst emote if thirst level is low and position it
        if (thirstLevel <= 30f)
        {
            thirstEmote.SetActive(true);
            thirstEmote.transform.localPosition = basePosition + new Vector3(startingOffset + emoteOffset * currentEmoteIndex, 0, 0);
            currentEmoteIndex++;
        }
        else
        {
            thirstEmote.SetActive(false);
        }

        // Show sick emote if cat has been hungry for too long
        if (timeSinceHungry > 30f)
        {
            sickEmote.SetActive(true);
            sickEmote.transform.localPosition = basePosition + new Vector3(startingOffset + emoteOffset * currentEmoteIndex, 0, 0);
            currentEmoteIndex++;
        }
        else
        {
            sickEmote.SetActive(false);
        }

        // Show dirty emote if cat has been unpetted for too long
        if (timeSinceUnpetted > 60f)
        {
            dirtyEmote.SetActive(true);
            dirtyEmote.transform.localPosition = basePosition + new Vector3(startingOffset + emoteOffset * currentEmoteIndex, 0, 0);
            currentEmoteIndex++;
        }
        else
        {
            dirtyEmote.SetActive(false);
        }
    }

    private void SaveCatStatus()
    {
        PlayerPrefs.SetFloat(catID + "_Hunger", hungerLevel);
        PlayerPrefs.SetFloat(catID + "_Affection", affectionLevel);
        PlayerPrefs.SetFloat(catID + "_Thirst", thirstLevel);
        PlayerPrefs.SetString(catID + "_LastUpdate", System.DateTime.Now.ToString()); // Store last update time
        PlayerPrefs.Save();
    }

    private void LoadCatStatus()
    {
        hungerLevel = PlayerPrefs.GetFloat(catID + "_Hunger", 100f);
        affectionLevel = PlayerPrefs.GetFloat(catID + "_Affection", 100f);
        thirstLevel = PlayerPrefs.GetFloat(catID + "_Thirst", 100f);

        // Check if there is a stored last update time
        if (PlayerPrefs.HasKey(catID + "_LastUpdate"))
        {
            System.DateTime lastUpdate = System.DateTime.Parse(PlayerPrefs.GetString(catID + "_LastUpdate"));
            float timeSinceLastUpdate = (float)(System.DateTime.Now - lastUpdate).TotalSeconds;

            // Update status based on elapsed time since last update
            hungerLevel -= timeSinceLastUpdate * decreaseRateHunger;
            affectionLevel -= timeSinceLastUpdate * decreaseRateAffection;
            thirstLevel -= timeSinceLastUpdate * decreaseRateThirst;
        }
    }

    public void FeedCat()
    {
        hungerLevel += 100f;
        SaveCatStatus();
    }

    public void TreatCat()
    {
        hungerLevel += 30f;
        SaveCatStatus();
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

}
