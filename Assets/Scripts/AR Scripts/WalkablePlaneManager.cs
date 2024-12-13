using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class WalkablePlaneManager : MonoBehaviour
{
    [SerializeField]
    private GameObject treatPrefab;
    [SerializeField]
    private GameObject feedPrefab;
    [SerializeField]
    private GameObject drinkPrefab;

    private List<GameObject> cats;
    private Camera arCamera;
    public bool canPlaceTreat = false;
    public bool canPlaceFeed = false;
    public bool canPlaceDrink = false;

    private Dictionary<GameObject, GameObject> catTreats = new Dictionary<GameObject, GameObject>();
    private Dictionary<GameObject, GameObject> catFeeds = new Dictionary<GameObject, GameObject>();
    private Dictionary<GameObject, GameObject> catDrinks = new Dictionary<GameObject, GameObject>();
    private HashSet<GameObject> activeEatingCats = new HashSet<GameObject>(); // Track cats that are currently eating or drinking

    private TMP_Text coinBalanceText;
    public const string CashKey = "PlayerCash";

    //GoalsManager goalsManager;
    [SerializeField] private GoalsManager goalsManager;
    [SerializeField] private GoalsManagerTier2 goalsManagerTier2;
    [SerializeField] private GoalsManagerTier3 goalsManagerTier3;
    private const string GoalsCounterKey = "GoalsCounter";

    private void Awake()
    {
        arCamera = Camera.main;

        if (treatPrefab == null) Debug.LogError("Treat prefab is not assigned!");
        if (feedPrefab == null) Debug.LogError("Feed prefab is not assigned!");
        if (drinkPrefab == null) Debug.LogError("Drink prefab is not assigned!");
        cats = new List<GameObject>();
    }

    private void Start()
    {
        cats = new List<GameObject>(GameObject.FindGameObjectsWithTag("Cat"));
        // Get a reference to the GoalsManager
        goalsManager = FindObjectOfType<GoalsManager>();



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

    void Update()
    {
        if (cats == null || cats.Count == 0)
        {
            cats = new List<GameObject>(GameObject.FindGameObjectsWithTag("Cat"));
        }

        if (cats.Count > 0)
        {
            HandleInput();
        }
    }

    private void HandleInput()
    {
#if UNITY_EDITOR
        HandleMouseInput();
#else
        HandleTouchInput();
#endif
    }

    private void HandleTouchInput()
    {
        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.isPressed)
        {
            Vector2 touchPosition = Touchscreen.current.primaryTouch.position.ReadValue();
            ProcessTouch(touchPosition);
        }
    }

    private void HandleMouseInput()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Vector2 mousePosition = Mouse.current.position.ReadValue();
            ProcessTouch(mousePosition);
        }
    }

    private void ProcessTouch(Vector2 position)
    {
        Ray ray = arCamera.ScreenPointToRay(position);
        Debug.DrawRay(ray.origin, ray.direction * 10, Color.red, 2.0f);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            GameObject hitObject = hit.collider.gameObject;

            if (cats.Contains(hitObject) && !activeEatingCats.Contains(hitObject))
            {
                GameObject selectedCat = hitObject;
                Debug.Log("Raycast hit the cat: " + selectedCat.name);

                CatBehavior catBehavior = selectedCat.GetComponent<CatBehavior>();
                if (catBehavior == null)
                {
                    Debug.LogError("CatBehavior component missing on: " + selectedCat.name);
                    return;
                }

                if (canPlaceTreat && !catTreats.ContainsKey(selectedCat))
                {
                    SpawnTreatInFrontOfCat(selectedCat);
                }
                else if (canPlaceFeed && !catFeeds.ContainsKey(selectedCat))
                {
                    SpawnFeedInFrontOfCat(selectedCat);
                }
                else if (canPlaceDrink && !catDrinks.ContainsKey(selectedCat))
                {
                    SpawnDrinkInFrontOfCat(selectedCat);
                }
            }
        }
        else
        {
            Debug.Log("Raycast did not hit anything.");
        }
    }

    private void SpawnTreatInFrontOfCat(GameObject selectedCat)
    {
        // Check if the cat is moving, eating, or drinking
        CatMover catMover = selectedCat.GetComponent<CatMover>();
        CatBehavior catBehavior = selectedCat.GetComponent<CatBehavior>();

        if (catMover != null && (catMover.isWalking || catBehavior.isEating || catBehavior.isDrinking))
        {
            Debug.Log("Cannot spawn treat while the cat is moving, eating, or drinking.");
            UIMessageManager.Instance.ShowMessage("Cannot spawn treat while the cat is moving, eating, or drinking.");
            return; // Stop the treat spawning if the cat is in any of these states
        }

        // Find the UI element by tag
        GameObject coinBalanceObject = GameObject.FindWithTag("Balance");
        coinBalanceText = coinBalanceObject.GetComponent<TMP_Text>();

        const int treatcost = 5; // feed cost

        // Parse the balance from the UI text
        int playerBalance = int.Parse(coinBalanceText.text);

        // condition
        if (playerBalance >= treatcost)
        {
            playerBalance -= treatcost; // Deduct coins
            coinBalanceText.text = playerBalance.ToString(); // Update the UI balance

            Vector3 spawnPosition = selectedCat.transform.position + selectedCat.transform.forward * 0.2f + Vector3.up * 0.1f;
            GameObject spawnedTreat = Instantiate(treatPrefab, spawnPosition, Quaternion.identity);
            catTreats[selectedCat] = spawnedTreat;

            CatStatus catStatus = selectedCat.GetComponent<CatStatus>();

            if (catBehavior != null)
            {
                activeEatingCats.Add(selectedCat); // Mark as eating
                catBehavior.TransitionToEating(spawnedTreat, 5f);

                catStatus.TreatCat();

                //Debug.Log("GOals counter ky value >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>" + PlayerPrefs.GetInt(GoalsCounterKey, 0));

                // Goals manager
                if (PlayerPrefs.GetInt(GoalsCounterKey, 0) == 1) {
                    goalsManager.IncrementTreatsGoal();  // Tier 1
                } 
                
                if (PlayerPrefs.GetInt(GoalsCounterKey, 0) == 2) {
                    goalsManagerTier2.IncrementTreatsGoal(); //Tier2
                }

                if (PlayerPrefs.GetInt(GoalsCounterKey, 0) == 3) {

                    goalsManagerTier3.IncrementTreatsGoal(); //Tier3
                }
            }

            StartCoroutine(DisableFeedAfterTime(spawnedTreat, selectedCat, 5f, catTreats));

            // Update the player's balance in PlayerPrefs
            int currentCash = PlayerPrefs.GetInt(CashKey, 0);
            currentCash -= treatcost;
            PlayerPrefs.SetInt(CashKey, currentCash);
            PlayerPrefs.Save();
        } else
        {
            Debug.Log("Not enough balance to feed!");
            UIMessageManager.Instance.ShowMessage("Not enough balance to feed!");
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayInsufficientFundsSFX(); // Play the click sound
            }
        }
    }


    private void SpawnFeedInFrontOfCat(GameObject selectedCat)
    {
        // Check if the cat is moving, eating, or drinking
        CatMover catMover = selectedCat.GetComponent<CatMover>();
        CatBehavior catBehavior = selectedCat.GetComponent<CatBehavior>();

        if (catMover != null && (catMover.isWalking || catBehavior.isEating || catBehavior.isDrinking))
        {
            Debug.Log("Cannot spawn feed while the cat is moving, eating, or drinking.");
            UIMessageManager.Instance.ShowMessage("Cannot spawn feed while the cat is moving, eating, or drinking.");
            return; // Stop feed spawning if the cat is in any of these states
        }

        // Find the UI element by tag
        GameObject coinBalanceObject = GameObject.FindWithTag("Balance");
        coinBalanceText = coinBalanceObject.GetComponent<TMP_Text>();

        const int feedingcost = 20; // feed cost

        // Parse the balance from the UI text
        int playerBalance = int.Parse(coinBalanceText.text);

        // condition
        if (playerBalance >= feedingcost)
        {
            playerBalance -= feedingcost; // Deduct coins
            coinBalanceText.text = playerBalance.ToString(); // Update the UI balance

            Vector3 spawnPosition = selectedCat.transform.position + selectedCat.transform.forward * 0.2f + Vector3.up * 0.1f;
            GameObject spawnedFeed = Instantiate(feedPrefab, spawnPosition, Quaternion.identity);
            catFeeds[selectedCat] = spawnedFeed;

            CatStatus catStatus = selectedCat.GetComponent<CatStatus>();

            if (catBehavior != null)
            {
                activeEatingCats.Add(selectedCat);
                catBehavior.TransitionToEating(spawnedFeed, 10f);
                catStatus.FeedCat();
            }

            StartCoroutine(DisableFeedAfterTime(spawnedFeed, selectedCat, 10f, catFeeds));

            // save
            int currentCash = PlayerPrefs.GetInt(CashKey, 0);
            currentCash -= feedingcost;
            PlayerPrefs.SetInt(CashKey, currentCash);
            PlayerPrefs.Save();
        }
        else
        {
            Debug.Log("Not enough balance to feed!");
            UIMessageManager.Instance.ShowMessage("Not enough balance to feed!");
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayInsufficientFundsSFX(); // Play the click sound
            }
        }
    }


    private void SpawnDrinkInFrontOfCat(GameObject selectedCat)
    {
        // Check if the cat is moving, eating, or drinking
        CatMover catMover = selectedCat.GetComponent<CatMover>();
        CatBehavior catBehavior = selectedCat.GetComponent<CatBehavior>();

        if (catMover != null && (catMover.isWalking || catBehavior.isEating || catBehavior.isDrinking))
        {
            Debug.Log("Cannot spawn drink while the cat is moving, eating, or drinking.");
            UIMessageManager.Instance.ShowMessage("Cannot spawn drink while the cat is moving, eating, or drinking.");
            return; // Stop drink spawning if the cat is in any of these states
        }

        // Find the UI element by tag
        GameObject coinBalanceObject = GameObject.FindWithTag("Balance");
        coinBalanceText = coinBalanceObject.GetComponent<TMP_Text>();

        const int drinkingCost = 10; // drinkingcost

        // Parse the balance from the UI text
        int playerBalance = int.Parse(coinBalanceText.text);

        // condition
        if (playerBalance >= drinkingCost)
        {

            playerBalance -= drinkingCost; // Deduct coins
            coinBalanceText.text = playerBalance.ToString(); // Update the UI balance

            Vector3 spawnPosition = selectedCat.transform.position + selectedCat.transform.forward * 0.2f + Vector3.up * 0.1f;
            GameObject spawnedDrink = Instantiate(drinkPrefab, spawnPosition, Quaternion.identity);
            catDrinks[selectedCat] = spawnedDrink;

            CatStatus catStatus = selectedCat.GetComponent<CatStatus>();

            if (catBehavior != null)
            {
                activeEatingCats.Add(selectedCat);
                catBehavior.TransitionToDrinking(spawnedDrink, 10f);
                catStatus.GiveWater();
            }

            StartCoroutine(DisableFeedAfterTime(spawnedDrink, selectedCat, 10f, catDrinks));

            // save
            int currentCash = PlayerPrefs.GetInt(CashKey, 0);
            currentCash -= drinkingCost;
            PlayerPrefs.SetInt(CashKey, currentCash);
            PlayerPrefs.Save();
        }
        else
        {
            Debug.Log("Not enough balance to transition to drinking!");
            UIMessageManager.Instance.ShowMessage("Not enough balance for drinks!");
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayInsufficientFundsSFX(); // Play the click sound
            }
        }
    }

    private IEnumerator DisableFeedAfterTime(GameObject feed, GameObject cat, float delay, Dictionary<GameObject, GameObject> dictionary)
    {
        yield return new WaitForSeconds(delay);

        if (feed == null)
        {
            Debug.LogWarning($"{cat.name}'s feed/drink object was already destroyed before coroutine completion.");
        }
        else
        {
            Destroy(feed);
            Debug.Log($"{cat.name} has finished eating/drinking. Feed/Drink object successfully destroyed.");
        }

        // Ensure cleanup from dictionary and activeEatingCats set
        if (dictionary.ContainsKey(cat))
        {
            dictionary.Remove(cat);
            Debug.Log($"{cat.name} removed from dictionary after finishing eating/drinking.");
        }
        else
        {
            Debug.LogWarning($"{cat.name} was not found in the dictionary when trying to remove it.");
        }

        if (activeEatingCats.Contains(cat))
        {
            activeEatingCats.Remove(cat);
            Debug.Log($"{cat.name} removed from activeEatingCats and is ready for new interactions.");
        }
        else
        {
            Debug.LogWarning($"{cat.name} was not found in activeEatingCats when trying to remove it.");
        }
    }


    private void OnDestroy()
    {
        Debug.Log($"{gameObject.name} is being destroyed.");
    }


    public void ToggleTreatPlacement()
    {
        canPlaceTreat = !canPlaceTreat;
        if (canPlaceTreat)
        {
            canPlaceFeed = false;
            canPlaceDrink = false;
        }
    }

    public void ToggleFeedPlacement()
    {
        canPlaceFeed = !canPlaceFeed;
        if (canPlaceFeed)
        {
            canPlaceTreat = false;
            canPlaceDrink = false;
        }
    }

    public void ToggleDrinkPlacement()
    {
        canPlaceDrink = !canPlaceDrink;
        if (canPlaceDrink)
        {
            canPlaceTreat = false;
            canPlaceFeed = false;
        }
    }

    public void AddCatToList(GameObject cat)
    {
        if (!cats.Contains(cat))
        {
            cats.Add(cat);
            Debug.Log("Added cat to list: " + cat.name);
        }
    }

    public void ClearSpawnedCats()
    {
        // Clear associated resources like treats, feed, and drinks
        foreach (var cat in cats)
        {
            if (catTreats.ContainsKey(cat))
            {
                Destroy(catTreats[cat]);
                catTreats.Remove(cat);
            }

            if (catFeeds.ContainsKey(cat))
            {
                Destroy(catFeeds[cat]);
                catFeeds.Remove(cat);
            }

            if (catDrinks.ContainsKey(cat))
            {
                Destroy(catDrinks[cat]);
                catDrinks.Remove(cat);
            }

            // Clear active eating/drinking status
            activeEatingCats.Remove(cat);

            // Destroy the cat itself
            Destroy(cat);
        }

        // Clear the list of cats
        cats.Clear();

        Debug.Log("All spawned cats and their associated objects have been cleared.");
    }


    public void ClearSpawnedObjects()
    {
        // Clear treats
        foreach (var kvp in catTreats)
        {
            if (kvp.Value != null)
            {
                Destroy(kvp.Value);
            }
        }
        catTreats.Clear();

        // Clear feeds
        foreach (var kvp in catFeeds)
        {
            if (kvp.Value != null)
            {
                Destroy(kvp.Value);
            }
        }
        catFeeds.Clear();

        // Clear drinks
        foreach (var kvp in catDrinks)
        {
            if (kvp.Value != null)
            {
                Destroy(kvp.Value);
            }
        }
        catDrinks.Clear();

        // Clear activeEatingCats set
        activeEatingCats.Clear();

        Debug.Log("All spawned objects (treats, feeds, drinks) have been cleared.");
    }


}
