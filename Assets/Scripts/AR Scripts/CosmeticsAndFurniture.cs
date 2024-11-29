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
    [SerializeField] private float xOffset = 187; // Amount to adjust the x position
    [SerializeField] private float hiddenXOffset = 100f; // X position when tab is moved to back
    [SerializeField] private float moveDuration = 0.5f; // Duration of the smooth transition

    public void ToggleComponents() {
        foreach (GameObject component in uiComponents) {
            component.SetActive(!component.activeSelf); // Toggle active state
        }
    }
    public void CloseComponents() {
        foreach (GameObject component in uiComponents) {
            if (component.activeSelf)  // Check if the component is currently open (active)
            {
                component.SetActive(false);  // Close it by setting it to inactive
            }
        }
    }
    public void BringFurnitureToFront() {
        if (Furniture != null && Cosmetics != null) {
            // Bring FurnitureTab to the front
            Furniture.transform.SetAsLastSibling();
            SetTabPosition(FurnitureTab, xOffset);
            SetTabPosition(CosmeticTab, hiddenXOffset);
        } else {
            Debug.LogWarning("FurnitureTab or CosmeticTab is not assigned.");
        }
    }

    public void BringCosmeticsToFront() {
        if (Furniture != null && Cosmetics != null) {
            // Bring CosmeticTab to the front
            Cosmetics.transform.SetAsLastSibling();
            SetTabPosition(CosmeticTab, xOffset);
            SetTabPosition(FurnitureTab, hiddenXOffset);
        } else {
            Debug.LogWarning("FurnitureTab or CosmeticTab is not assigned.");
        }
    }

    private void SetTabPosition(GameObject tab, float targetXPos) {
        RectTransform tabTransform = tab.GetComponent<RectTransform>();
        StartCoroutine(SmoothMove(tabTransform, targetXPos));
    }

    private IEnumerator SmoothMove(RectTransform tabTransform, float targetXPos) {
        float elapsedTime = 0f;
        Vector2 startPos = tabTransform.anchoredPosition;
        Vector2 targetPos = new Vector2(targetXPos, startPos.y); // Change only the x-coordinate

        while (elapsedTime < moveDuration) {
            tabTransform.anchoredPosition = Vector2.Lerp(startPos, targetPos, elapsedTime / moveDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        tabTransform.anchoredPosition = targetPos; // Ensure it ends at the exact target position
    }
}
