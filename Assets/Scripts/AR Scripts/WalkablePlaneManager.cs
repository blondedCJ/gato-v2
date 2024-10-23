using System.Collections;
using System.Collections.Generic; // Import for using List
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

    private List<GameObject> cats; // List to store all cats
    private Camera arCamera;
    private bool canPlaceTreat = false;
    private bool canPlaceFeed = false;
    private bool canPlaceDrink = false;

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
        // Find all cats in the scene with the tag "Cat"
        cats = new List<GameObject>(GameObject.FindGameObjectsWithTag("Cat"));
    }

    void Update()
    {
        // Refresh the list of cats if needed (e.g., if cats can be spawned dynamically)
        if (cats == null || cats.Count == 0)
        {
            cats = new List<GameObject>(GameObject.FindGameObjectsWithTag("Cat"));
        }

        // Handle input if there are cats in the scene
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

    private void AddCatsInScene()
    {
        GameObject[] initialCats = GameObject.FindGameObjectsWithTag("Cat");
        foreach (GameObject cat in initialCats)
        {
            AddCatToList(cat);
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
    private void ProcessTouch(Vector2 position)
    {
        Ray ray = arCamera.ScreenPointToRay(position);
        Debug.DrawRay(ray.origin, ray.direction * 10, Color.red, 2.0f); // Visualize raycast

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            GameObject hitObject = hit.collider.gameObject;

            if (cats.Contains(hitObject))
            {
                GameObject selectedCat = hitObject; // The cat that was hit
                Debug.Log("Raycast hit the cat: " + selectedCat.name);

                // Ensure CatBehavior is present
                CatBehavior catBehavior = selectedCat.GetComponent<CatBehavior>();
                if (catBehavior == null)
                {
                    Debug.LogError("CatBehavior component missing on: " + selectedCat.name);
                    return;
                }

                // Perform actions based on the selected cat
                if (canPlaceTreat)
                {
                    Debug.Log("Spawning treat in front of: " + selectedCat.name);
                    SpawnTreatInFrontOfCat(selectedCat);
                }
                else if (canPlaceFeed)
                {
                    Debug.Log("Spawning feed in front of: " + selectedCat.name);
                    SpawnFeedInFrontOfCat(selectedCat);
                }
                else if (canPlaceDrink)
                {
                    Debug.Log("Spawning drink in front of: " + selectedCat.name);
                    SpawnDrinkInFrontOfCat(selectedCat);
                }
            }
            else
            {
                Debug.Log("Raycast hit: " + hitObject.name);
            }
        }
        else
        {
            Debug.Log("Raycast did not hit anything.");
        }
    }

    private void SpawnTreatInFrontOfCat(GameObject selectedCat)
    {
        Vector3 spawnPosition = selectedCat.transform.position + selectedCat.transform.forward * 0.25f + Vector3.up * 0.1f;
        GameObject spawnedTreat = Instantiate(treatPrefab, spawnPosition, Quaternion.identity);

        if (spawnedTreat != null)
        {
            Debug.Log("Treat instantiated successfully at: " + spawnedTreat.transform.position);

            CatBehavior catBehavior = selectedCat.GetComponent<CatBehavior>();
            CatStatus catStatus = selectedCat.GetComponent<CatStatus>(); // Get the CatStatus

            if (catBehavior != null)
            {
                float duration = 5f; // Set the duration for the eating animation
                catBehavior.TransitionToEating(spawnedTreat, duration);
                catStatus.TreatCat(); // Update the hunger level
            }
            else
            {
                Debug.LogError("CatBehavior not found on the cat object!");
            }
            StartCoroutine(DisableFeedAfterTime(spawnedTreat, 3f));
        }
        else
        {
            Debug.LogError("Failed to instantiate treat.");
        }
    }


    private void SpawnFeedInFrontOfCat(GameObject selectedCat)
    {
        Vector3 spawnPosition = selectedCat.transform.position + selectedCat.transform.forward * 0.25f + Vector3.up * 0.1f;
        GameObject spawnedFeed = Instantiate(feedPrefab, spawnPosition, Quaternion.identity);

        if (spawnedFeed != null)
        {
            CatBehavior catBehavior = selectedCat.GetComponent<CatBehavior>();
            CatStatus catStatus = selectedCat.GetComponent<CatStatus>(); // Get the CatStatus

            if (catBehavior != null)
            {
                float duration = 10f; // Set the duration for the eating animation
                catBehavior.TransitionToEating(spawnedFeed, duration);
                catStatus.FeedCat(); // Update the hunger level fully
            } 
            else
            {
                Debug.LogError("CatBehavior not found on the cat object!");
            }
            StartCoroutine(DisableFeedAfterTime(spawnedFeed, 10f));
        }
        else
        {
            Debug.LogError("Failed to instantiate feed.");
        }
    }


    private void SpawnDrinkInFrontOfCat(GameObject selectedCat)
    {
        Vector3 spawnPosition = selectedCat.transform.position + selectedCat.transform.forward * 0.25f + Vector3.up * 0.1f;
        GameObject spawnedDrink = Instantiate(drinkPrefab, spawnPosition, Quaternion.identity);

        if (spawnedDrink != null)
        {
            CatBehavior catBehavior = selectedCat.GetComponent<CatBehavior>();
            CatStatus catStatus = selectedCat.GetComponent<CatStatus>(); // Get the CatStatus

            if (catBehavior != null)
            {
                float duration = 10f; // Set the duration for the drinking animation
                catBehavior.TransitionToDrinking(spawnedDrink, duration);
                
            }
            else
            {
                Debug.LogError("CatBehavior not found on the cat object!");
            }
            catStatus.GiveWater(); // Update the thirst level
        }
        else
        {
            Debug.LogError("Failed to instantiate drink.");
        }
    }


    // Coroutine to disable feed after a specified time
    private IEnumerator DisableFeedAfterTime(GameObject feed, float delay)
    {
        yield return new WaitForSeconds(delay);

        if (feed != null)
        {
            Destroy(feed);
            Debug.Log("Feed has been removed after " + delay + " seconds.");
        }
    }

    public void ToggleTreatPlacement()
    {
        canPlaceTreat = !canPlaceTreat;
        if (canPlaceTreat)
        {
            canPlaceFeed = false;
            canPlaceDrink = false;
            Debug.Log("Treat placement enabled.");
        }
        else
        {
            Debug.Log("Treat placement disabled.");
        }
    }

    public void ToggleFeedPlacement()
    {
        canPlaceFeed = !canPlaceFeed;
        if (canPlaceFeed)
        {
            canPlaceTreat = false;
            canPlaceDrink = false;
            Debug.Log("Feed placement enabled.");
        }
        else
        {
            Debug.Log("Feed placement disabled.");
        }
    }

    public void ToggleDrinkPlacement()
    {
        canPlaceDrink = !canPlaceDrink;
        if (canPlaceDrink)
        {
            canPlaceTreat = false;
            canPlaceFeed = false;
            Debug.Log("Drink placement enabled.");
        }
        else
        {
            Debug.Log("Drink placement disabled.");
        }
    }
}
