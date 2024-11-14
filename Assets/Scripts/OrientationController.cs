using UnityEngine;

public class OrientationController : MonoBehaviour
{
    void Start()
    {
        Screen.orientation = ScreenOrientation.Portrait; // or ScreenOrientation.LandscapeRight
    }
}
