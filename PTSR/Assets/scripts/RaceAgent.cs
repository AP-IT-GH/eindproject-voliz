using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class RaceAgent : Agent
{
    public Transform[] checkpoints; // Array of checkpoints
    private int nextCheckpointIndex = 0;
    public float moveSpeed = 2.0f; // Movement speed
    public float turnSpeed = 200.0f; // Turning speed
    public float maxTurnAngle = 45.0f; // Maximum allowable turn angle per step

    private Vector3 initialPosition; // Initial position of the agent
    private Quaternion initialRotation; // Initial rotation of the agent

    public RayPerceptionSensorComponent3D raySensor; // Reference to Ray Perception Sensor

    // Y-coordinate below which the agent is considered to have fallen off
    public float fallHeight = -2.0f;

    // Avoidance variables
    public float avoidSpeed = 1.0f; // Speed at which to avoid obstacles
    public float collisionRadius = 0.5f; // Radius to detect obstacles
    public LayerMask obstacleMask; // Layer mask for obstacles

    // Penalty variables
    public float collisionPenalty = -0.4f; // Penalty for collision with obstacles
    public float turnPenalty = -0.01f; // Penalty for turning

    private bool isAvoidingObstacle = false; // Flag to check if avoiding an obstacle

    public override void Initialize()
    {
        // Save the initial position and rotation of the agent
        initialPosition = transform.localPosition;
        initialRotation = transform.localRotation;

        // Find the Ray Perception Sensor in the agent's GameObject
        raySensor = GetComponent<RayPerceptionSensorComponent3D>();
    }

    public override void OnEpisodeBegin()
    {
        // Reset the agent and environment at the start of an episode
        nextCheckpointIndex = 0;

        // Reset the agent's position and rotation
        transform.localPosition = initialPosition;
        transform.localRotation = initialRotation;

        // Optionally reset any Rigidbody or other physics properties if used
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        isAvoidingObstacle = false; // Reset the avoidance flag
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // Collect observations from the environment (e.g., positions of checkpoints, distance to next checkpoint)
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(checkpoints[nextCheckpointIndex].localPosition);

        // Additional observations: distance and angle to next checkpoint
        Vector3 toNextCheckpoint = checkpoints[nextCheckpointIndex].localPosition - transform.localPosition;
        sensor.AddObservation(toNextCheckpoint.magnitude); // Distance to next checkpoint
        sensor.AddObservation(Vector3.Dot(transform.forward, toNextCheckpoint.normalized)); // Alignment with the next checkpoint direction

        // Ray Perception observations are automatically collected by the sensor component
    }

    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        // Get the actions from the neural network
        var continuousActions = actionBuffers.ContinuousActions;

        // Convert actions to movement and turning
        float moveZ = Mathf.Clamp(continuousActions[0], 0f, 1f); // Ensure moveZ is positive for forward movement
        float turn = Mathf.Clamp(continuousActions[1], -1f, 1f); // Turn value between -1 and 1

        // Calculate movement and turning
        Vector3 forwardMovement = transform.forward * moveZ * moveSpeed * Time.deltaTime;
        float turnAngle = turn * maxTurnAngle * Time.deltaTime;

        // Apply movement and turning to the agent
        transform.localPosition += forwardMovement;
        transform.Rotate(Vector3.up, turnAngle);

        // Apply penalty for turning
        if (Mathf.Abs(turnAngle) > 0.01f)
        {
            AddReward(turnPenalty);
        }

        // Check for collisions and avoid obstacles
        if (isAvoidingObstacle)
        {
            // Apply avoidance movement
            forwardMovement = AvoidObstacle();
            transform.localPosition += forwardMovement;
        }

        // Add a small negative reward for each step to encourage faster completion
        AddReward(-0.001f);

        // Check if the agent has fallen off
        if (transform.localPosition.y < fallHeight)
        {
            // Agent has fallen off, reset the episode
            SetReward(-1.0f); // Penalize for falling off
            EndEpisode();
        }
    }

    private bool IsColliding()
    {
        // Check if there are collisions with obstacles using SphereCast or other methods
        RaycastHit hit;
        Vector3 raycastDirection = transform.forward;
        if (Physics.SphereCast(transform.position, collisionRadius, raycastDirection, out hit, 1.0f, obstacleMask))
        {
            // Check if the collided object is not a checkpoint
            if (!IsCheckpoint(hit.collider.gameObject))
            {
                return true;
            }
        }
        return false;
    }

    private bool IsCheckpoint(GameObject obj)
    {
        // Check if the collided object is a checkpoint
        foreach (Transform checkpoint in checkpoints)
        {
            if (obj.transform.IsChildOf(checkpoint))
            {
                return true;
            }
        }
        return false;
    }

    private Vector3 AvoidObstacle()
    {
        // Steer away from obstacles using a simple right-hand rule
        Vector3 avoidDirection = transform.right;
        return avoidDirection * avoidSpeed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Wall"))
        {
            // Set avoidance flag
            isAvoidingObstacle = true;

            // Apply penalty for collision
            AddReward(collisionPenalty);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Wall"))
        {
            // Clear avoidance flag
            isAvoidingObstacle = false;
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        // Use keyboard input for testing the agent
        var continuousActionsOut = actionsOut.ContinuousActions;
        continuousActionsOut[0] = Input.GetAxis("Vertical");
        continuousActionsOut[1] = Input.GetAxis("Horizontal");
    }

    // Method to handle when a checkpoint is reached
    public void CheckpointReached(Checkpoint checkpoint)
    {
        // Ensure the checkpoint is the next expected one
        if (checkpoint == checkpoints[nextCheckpointIndex].GetComponent<Checkpoint>())
        {
            AddReward(1.0f); // Reward for reaching the checkpoint
            nextCheckpointIndex = (nextCheckpointIndex + 1) % checkpoints.Length; // Move to the next checkpoint
        }
    }
}
