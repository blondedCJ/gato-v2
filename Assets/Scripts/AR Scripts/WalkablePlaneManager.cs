using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class WalkablePlaneManager : MonoBehaviour
{
    [SerializeField]
    private GameObject treatPrefab; // Reference to the treat prefab
    [SerializeField]
    private GameObject feedPrefab;  // Reference to the feed prefab
    [SerializeField]
    private GameObject drinkPrefab; // Reference to the drink prefab

    private GameObject cat; // Reference to the cat
    private Camera arCamera;
    private bool canPlaceTreat = false;
    private bool canPlaceFeed = false;
    private bool canPlaceDrink = false; // New flag for drink placement

    private void Awake()
    {
        arCamera = Camera.main;

        if (treatPrefab == null)
        {
            Debug.LogError("Treat prefab is not assigned!");
        }

        if (feedPrefab == null)
        {
            Debug.LogError("Feed prefab is not assigned!");
        }

        if (drinkPrefab == null) // Check for drink prefab
        {
            Debug.LogError("Drink prefab is not assigned!");
        }
    }

    void Update()
    {
        // Dynamically find the cat GameObject by tag
        if (cat == null)
        {
            cat = GameObject.FindWithTag("Cat");
        }

        // Allow input handling only if the cat exists and placement is enabled
        if (cat != null)
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

    // Method to handle touch input for mobile devices
    private void HandleTouchInput()
    {
        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.isPressed)
        {
            Vector2 touchPosition = Touchscreen.current.primaryTouch.position.ReadValue();
            ProcessTouch(touchPosition);
        }
    }

    // Method to handle mouse input for Unity Editor testing
    private void HandleMouseInput()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Vector2 mousePosition = Mouse.current.position.ReadValue();
            ProcessTouch(mousePosition);
        }
    }

    // Method to process touch/mouse input
    private void ProcessTouch(Vector2 position)
    {
        Ray ray = arCamera.ScreenPointToRay(position);
        Debug.DrawRay(ray.origin, ray.direction * 10, Color.red, 2.0f); // Visualize raycast

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            // Check if the ray hit the cat
            if (hit.collider.gameObject == cat)
            {
                Debug.Log("Raycast hit the cat. Spawning item.");
                if (canPlaceTreat)
                {
                    SpawnTreatInFrontOfCat();
                }
                else if (canPlaceFeed)
                {
                    SpawnFeedInFrontOfCat();
                }
                else if (canPlaceDrink) // Check for drink placement
                {
                    SpawnDrinkInFrontOfCat();
                }
            }
            else
            {
                Debug.Log("Raycast hit: " + hit.collider.gameObject.name);
            }
        }
        else
        {
            Debug.Log("Raycast did not hit anything.");
        }
    }

    // Method to spawn the treat in front of the cat
    private void SpawnTreatInFrontOfCat()
    {
        Vector3 spawnPosition = cat.transform.position + cat.transform.forward * 0.25f + Vector3.up * 0.1f;

        Debug.Log("Spawning treat at position: " + spawnPosition);
        GameObject spawnedTreat = Instantiate(treatPrefab, spawnPosition, Quaternion.identity);

        if (spawnedTreat != null)
        {
            Debug.Log("Treat instantiated successfully at: " + spawnedTreat.transform.position);
            CatBehavior catBehavior = cat.GetComponent<CatBehavior>();
            if (catBehavior != null)
            {
                float duration = 10f; // Set the duration for the eating animation
                catBehavior.TransitionToEating(spawnedTreat, duration);
            }
            else
            {
                Debug.LogError("CatBehavior not found on the cat object!");
            }
        }
        else
        {
            Debug.LogError("Failed to instantiate treat.");
        }
    }

    // Method to spawn the feed in front of the cat
    private void SpawnFeedInFrontOfCat()
    {
        Vector3 spawnPosition = cat.transform.position + cat.transform.forward * 0.25f + Vector3.up * 0.1f;

        Debug.Log("Spawning feed at position: " + spawnPosition);
        GameObject spawnedFeed = Instantiate(feedPrefab, spawnPosition, Quaternion.identity);

        if (spawnedFeed != null)
        {
            Debug.Log("Feed instantiated successfully at: " + spawnedFeed.transform.position);
            CatBehavior catBehavior = cat.GetComponent<CatBehavior>();
            if (catBehavior != null)
            {
                float duration = 3f; // Set the duration for the eating animation
                catBehavior.TransitionToEating(spawnedFeed, duration);
            }
            else
            {
                Debug.LogError("CatBehavior not found on the cat object!");
            }
            StartCoroutine(DisableFeedAfterTime(spawnedFeed, 3f));
        }
        else
        {
            Debug.LogError("Failed to instantiate feed.");
        }
    }

    // New method to spawn the drink in front of the cat
    private void SpawnDrinkInFrontOfCat()
    {
        Vector3 spawnPosition = cat.transform.position + cat.transform.forward * 0.25f + Vector3.up * 0.1f;

        // Log the spawn position
        Debug.Log("Spawning drink at position: " + spawnPosition);

        // Instantiate the drink prefab at the spawn position
        GameObject spawnedDrink = Instantiate(drinkPrefab, spawnPosition, Quaternion.identity);

        // Check if the drink was instantiated successfully
        if (spawnedDrink != null)
        {
            Debug.Log("Drink instantiated successfully at: " + spawnedDrink.transform.position);

            // Call the method in CatBehavior to trigger the drinking animation with duration
            CatBehavior catBehavior = cat.GetComponent<CatBehavior>();
            if (catBehavior != null)
            {
                float duration = 10f; // Set the duration for the drinking animation
                catBehavior.TransitionToDrinking(spawnedDrink, duration); // Call the drinking method with duration
            }
            else
            {
                Debug.LogError("CatBehavior not found on the cat object!");
            }
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
            Debug.Log("Feed has been removed after 3 seconds.");
        }
    }

    // New coroutine to disable drink after a specified time
    private IEnumerator DisableDrinkAfterTime(GameObject drink, float delay)
    {
        yield return new WaitForSeconds(delay);

        if (drink != null)
        {
            Destroy(drink);
            Debug.Log("Drink has been removed after 10 seconds.");
        }
    }

    // Method to enable treat placement when the treat button is clicked
    public void EnableTreatPlacement()
    {
        canPlaceTreat = true;
        canPlaceFeed = false;
        canPlaceDrink = false; // Disable drink placement
        Debug.Log("Treat placement enabled.");
    }

    // Method to enable feed placement when the feed button is clicked
    public void EnableFeedPlacement()
    {
        canPlaceFeed = true;
        canPlaceTreat = false;
        canPlaceDrink = false; // Disable drink placement
        Debug.Log("Feed placement enabled.");
    }

    // New method to enable drink placement when the drink button is clicked
    public void EnableDrinkPlacement()
    {
        canPlaceDrink = true; // Enable drink placement
        canPlaceTreat = false;
        canPlaceFeed = false; // Disable feed placement
        Debug.Log("Drink placement enabled.");
    }

    // Method to disable treat placement
    public void DisableTreatPlacement()
    {
        canPlaceTreat = false;
        Debug.Log("Treat placement disabled.");
    }

    // Method to disable feed placement
    public void DisableFeedPlacement()
    {
        canPlaceFeed = false;
        Debug.Log("Feed placement disabled.");
    }

    // New method to disable drink placement
    public void DisableDrinkPlacement()
    {
        canPlaceDrink = false;
        Debug.Log("Drink placement disabled.");
    }
}
