using UnityEngine;

public class OrientationController : MonoBehaviour
{
    public GameObject targetObject; // The GameObject to monitor for activity

    void Start()
    {
        Screen.orientation = ScreenOrientation.Portrait; // or ScreenOrientation.LandscapeRight
    }

    void Update()
    {
        // Check if the target GameObject is active in the scene
        if (targetObject.activeInHierarchy)
        {
            // If the target object is active, switch the screen to landscape
            Screen.orientation = ScreenOrientation.LandscapeLeft;
        }
        else
        {
            // If the target object is not active, switch the screen to portrait
            Screen.orientation = ScreenOrientation.Portrait;
        }
    }

}
