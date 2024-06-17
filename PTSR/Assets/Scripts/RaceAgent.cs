using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class RaceAgent : Agent
{
    public Transform[] checkpoints; 
    private int nextCheckpointIndex = 0;
    public float moveSpeed = 2.0f; 
    public float turnSpeed = 200.0f; 
    public float maxTurnAngle = 45.0f; 

    private Vector3 initialPosition; 
    private Quaternion initialRotation; 

    public RayPerceptionSensorComponent3D raySensor; 

    public float fallHeight = -2.0f;

    // Avoidance variables
    public float avoidSpeed = 1.0f; 
    public float collisionRadius = 0.5f; 
    public LayerMask obstacleMask; 

    // Penalty variables
    public float collisionPenalty = -0.4f; 
    public float turnPenalty = -0.01f; 

    private bool isAvoidingObstacle = false; 

    private Vector3 lastPosition;
    private int stuckSteps = 0;
    public int maxStuckSteps = 50; 

    public int maxStepsPerEpisode = 5000; 
    private int steps = 0;

    public override void Initialize()
    {
        initialPosition = transform.localPosition;
        initialRotation = transform.localRotation;

        // Find the Ray Perception Sensor in the agent's GameObject
        raySensor = GetComponent<RayPerceptionSensorComponent3D>();
    }

    public override void OnEpisodeBegin()
    {
        nextCheckpointIndex = 0;

        // Reset the agent's position and rotation
        transform.localPosition = initialPosition;
        transform.localRotation = initialRotation;

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        isAvoidingObstacle = false; 
        lastPosition = transform.localPosition; 
        stuckSteps = 0; 
        steps = 0; 
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(checkpoints[nextCheckpointIndex].localPosition);

        Vector3 toNextCheckpoint = checkpoints[nextCheckpointIndex].localPosition - transform.localPosition;
        sensor.AddObservation(toNextCheckpoint.magnitude); 
        sensor.AddObservation(Vector3.Dot(transform.forward, toNextCheckpoint.normalized)); 

    }

    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        // Increment the step counter
        steps++;

        var continuousActions = actionBuffers.ContinuousActions;

        float moveZ = Mathf.Clamp(continuousActions[0], 0f, 1f); 
        float turn = Mathf.Clamp(continuousActions[1], -1f, 1f); 

        Vector3 forwardMovement = transform.forward * moveZ * moveSpeed * Time.deltaTime;
        float turnAngle = turn * maxTurnAngle * Time.deltaTime;

        transform.localPosition += forwardMovement;
        transform.Rotate(Vector3.up, turnAngle);

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

        AddReward(-0.001f);

      
        if (transform.localPosition.y < fallHeight)
        {
           
            SetReward(-1.0f); 
            EndEpisode();
        }

        // Stuck detection logic
        if (Vector3.Distance(transform.localPosition, lastPosition) < 0.01f)
        {
            stuckSteps++;
            if (stuckSteps > maxStuckSteps)
            {
               
                SetReward(-1.0f); 
                EndEpisode();
            }
        }
        else
        {
            stuckSteps = 0; // Reset the stuck steps counter if the agent is moving
        }
        lastPosition = transform.localPosition; 

        
        if (steps > maxStepsPerEpisode)
        {
            EndEpisode(); 
        }
    }

    private bool IsColliding()
    {
        RaycastHit hit;
        Vector3 raycastDirection = transform.forward;
        if (Physics.SphereCast(transform.position, collisionRadius, raycastDirection, out hit, 1.0f, obstacleMask))
        {
            if (!IsCheckpoint(hit.collider.gameObject))
            {
                return true;
            }
        }
        return false;
    }

    private bool IsCheckpoint(GameObject obj)
    {
        
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
        RaycastHit hit;
        Vector3 raycastDirection = transform.forward;
        if (Physics.SphereCast(transform.position, collisionRadius, raycastDirection, out hit, 1.0f, obstacleMask))
        {
            Vector3 avoidDirection = Vector3.zero;
            avoidDirection = (transform.position - hit.point).normalized;
            avoidDirection.y = 0; 
            return avoidDirection * avoidSpeed * Time.deltaTime;
        }
        return Vector3.zero;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Wall"))
        {
            isAvoidingObstacle = true;

            AddReward(collisionPenalty);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Wall"))
        {
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
     
        if (checkpoint == checkpoints[nextCheckpointIndex].GetComponent<Checkpoint>())
        {
            AddReward(1.0f); 
            nextCheckpointIndex = (nextCheckpointIndex + 1) % checkpoints.Length; // Move to the next checkpoint
        }
    }
}
