using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleUIComponents : MonoBehaviour
{
    [SerializeField] private GameObject[] uiComponents; // Array for UI components to toggle
    [SerializeField] private GameObject Furniture;
    [SerializeField] private GameObject Cosmetics;
    [SerializeField] private GameObject FurnitureTab;
    [SerializeField] private GameObject CosmeticTab;
    [SerializeField] private float yOffset = -1156; // Amount to adjust the y position\
    [SerializeField] private float hiddenYOffset = -1197f; // Y position when tab is moved to back
    [SerializeField] private float moveDuration = 0.5f; // Duration of the smooth transition

    public void ToggleComponents()
    {
        foreach (GameObject component in uiComponents)
        {
            component.SetActive(!component.activeSelf); // Toggle active state
        }
    }
    public void BringFurnitureToFront()
    {
        if (Furniture != null && Cosmetics != null)
        {
            // Bring FurnitureTab to the front
            Furniture.transform.SetAsLastSibling();
            SetTabPosition(FurnitureTab, yOffset);
            SetTabPosition(CosmeticTab, hiddenYOffset);
        }
        else
        {
            Debug.LogWarning("FurnitureTab or CosmeticTab is not assigned.");
        }
    }

    public void BringCosmeticsToFront()
    {
        if (Furniture != null && Cosmetics != null)
        {
            // Bring CosmeticTab to the front
            Cosmetics.transform.SetAsLastSibling();
            SetTabPosition(CosmeticTab, yOffset);
            SetTabPosition(FurnitureTab, hiddenYOffset);
        }
        else
        {
            Debug.LogWarning("FurnitureTab or CosmeticTab is not assigned.");
        }
    }

    private void SetTabPosition(GameObject tab, float targetYPos)
    {
        RectTransform tabTransform = tab.GetComponent<RectTransform>();
        StartCoroutine(SmoothMove(tabTransform, targetYPos));
    }

    private IEnumerator SmoothMove(RectTransform tabTransform, float targetYPos)
    {
        float elapsedTime = 0f;
        Vector2 startPos = tabTransform.anchoredPosition;
        Vector2 targetPos = new Vector2(startPos.x, targetYPos);

        while (elapsedTime < moveDuration)
        {
            tabTransform.anchoredPosition = Vector2.Lerp(startPos, targetPos, elapsedTime / moveDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        tabTransform.anchoredPosition = targetPos; // Ensure it ends at the exact target position
    }

}
