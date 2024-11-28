using UnityEngine;

public class FallingObject : MonoBehaviour
{
    public float fallSpeed; // Speed at which the object falls
    public Vector3 rotationSpeed; // Speed of rotation for the object

    private void Start()
    {
        // Assign random rotation speed
        rotationSpeed = new Vector3(
            Random.Range(-50f, 50f),
            Random.Range(-50f, 50f),
            Random.Range(-50f, 50f)
        );
    }

    private void Update()
    {
        // Move the object down
        transform.position += Vector3.down * fallSpeed * Time.deltaTime;

        // Rotate the object
        transform.Rotate(rotationSpeed * Time.deltaTime);
    }
}
