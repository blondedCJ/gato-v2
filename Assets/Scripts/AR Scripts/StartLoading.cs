using UnityEngine;
using UnityEngine.UI;

public class LoadingScreenTest : MonoBehaviour
{
    public GameObject firstLoadingScreen; // Assign in the Inspector
    public Slider firstLoadingSlider;     // Assign in the Inspector
    public GameObject secondLoadingScreen; // Assign in the Inspector
    public Slider secondLoadingSlider;     // Assign in the Inspector

    private FakeLoadingScreen fakeLoadingScreen;

    private void Start()
    {
        fakeLoadingScreen = GetComponent<FakeLoadingScreen>();
    }

    public void StartFirstLoading()
    {
        fakeLoadingScreen.StartLoading(false, firstLoadingScreen, firstLoadingSlider);
    }

    public void StartSecondLoading()
    {
        fakeLoadingScreen.StartLoading(true, secondLoadingScreen, secondLoadingSlider);
    }
}
