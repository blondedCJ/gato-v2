using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuButtons : MonoBehaviour
{
    
    public void StartButton(int index)
    {
        SceneManager.LoadScene(index);
    }

    public void GoalsButton()
    {

    }

    public void SettingsButton()
    {

    }

    public void Button()
    {

    }

    public void Exit()
    {
        Debug.Log("Exiting the game...");

#if UNITY_EDITOR
        // Stops the play mode in the Unity editor.
        UnityEditor.EditorApplication.isPlaying = false;
#else
        // Exits the application in a built game.
        Application.Quit();
#endif
    }

}
