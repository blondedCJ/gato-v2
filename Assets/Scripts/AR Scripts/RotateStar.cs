using UnityEngine;

public class RotateStar : MonoBehaviour
{
    public float rotationSpeed = 100f; // Speed of rotation in degrees per second

    void Update()
    {
        // Rotate the star around its local Y-axis
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime, Space.Self);
    }
}
