using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 10f;
    public float turnSpeed = 100f;
    public float driftFactor = 0.95f;

    private float accelerationInput;
    private float steeringInput;

    void Update()
    {
        // Get input for acceleration and steering
        accelerationInput = Input.GetAxis("Vertical");
        steeringInput = Input.GetAxis("Horizontal");

        // Calculate the forward movement
        Vector3 forwardMovement = transform.forward * accelerationInput * moveSpeed * Time.deltaTime;

        // Apply the forward movement
        transform.Translate(forwardMovement, Space.World);

        // Calculate the turn angle
        float rotationAngle = steeringInput * turnSpeed * Time.deltaTime * (moveSpeed / 10f);

        // Apply the rotation
        transform.Rotate(Vector3.up, rotationAngle);

        // Apply drift factor
        Vector3 forwardVelocity = transform.forward * Vector3.Dot(transform.forward, GetComponent<Rigidbody>().velocity);
        Vector3 rightVelocity = transform.right * Vector3.Dot(transform.right, GetComponent<Rigidbody>().velocity);
        GetComponent<Rigidbody>().velocity = forwardVelocity + rightVelocity * driftFactor;
    }
}
