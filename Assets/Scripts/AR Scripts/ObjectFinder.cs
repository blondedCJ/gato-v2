using UnityEngine;

public class ObjectFinder : MonoBehaviour
{
    public GameObject targetObject;      // Assign the GameObject you want to check in the Inspector
    public GameObject nameYourCatObject; // Assign the "Name your Cat" GameObject in the Inspector


    private void Update()
    {
        if (targetObject == null || nameYourCatObject == null)
        {
            return;
        }

        // Check if the target GameObject is disabled
        if (!targetObject.activeInHierarchy)
        {
            // Enable the "Name your Cat" GameObject
            nameYourCatObject.SetActive(true);
        }

    }
}
