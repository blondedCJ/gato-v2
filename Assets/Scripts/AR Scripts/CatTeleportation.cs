using UnityEngine;

public class CatTeleportation : MonoBehaviour
{
    public float boundaryX = 10f; // Horizontal boundary (distance from the center)

    void Update()
    {
        CheckBoundary();
    }

    private void CheckBoundary()
    {
        Vector3 position = transform.position;

        // If the cat goes beyond the right boundary, teleport to the left
        if (position.x > boundaryX)
        {
            position.x = -boundaryX;
        }
        // If the cat goes beyond the left boundary, teleport to the right
        else if (position.x < -boundaryX)
        {
            position.x = boundaryX;
        }

        transform.position = position;
    }
}
