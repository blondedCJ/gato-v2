using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearData : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ResetUserOwnedCats()
    {
        PlayerPrefs.DeleteAll(); // This will delete all PlayerPrefs data
        PlayerPrefs.Save(); // Ensure changes are saved
        Debug.Log("All PlayerPrefs data has been reset.");
    }

}
