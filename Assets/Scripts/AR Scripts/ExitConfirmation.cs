using UnityEngine;
using UnityEngine.UI;

public class ShowGameObjectOnClick : MonoBehaviour
{
    // Reference to the button
    public Button button;

    // Reference to the GameObject to be revealed
    public GameObject objectToReveal;

    // Start is called before the first frame update
    void Start() {
        // Ensure the object is hidden at the start
        //objectToReveal.SetActive(false);

        // Add listener to the button to reveal the object when clicked
        button.onClick.AddListener(RevealObject);
    }

    // Function to reveal the GameObject
    private void RevealObject() {
        // Set the GameObject to active (unhide it)
        objectToReveal.SetActive(true);
    }
}
