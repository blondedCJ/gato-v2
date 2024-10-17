using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrickUIManager : MonoBehaviour
{
    public CatManager catManager;

    public void OnPlayDeadButtonPressed()
    {
        catManager.PerformTrick("PlayDead");
    }

    public void OnJumpButtonPressed()
    {
        catManager.PerformTrick("Jump");
    }
}
