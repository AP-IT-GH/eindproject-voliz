using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class BallAgent : Agent
{
    public LayerMask groundLayer;
    public LayerMask sideLayer;
    public float rewardPerCheckpoint = 1f;
    public float penaltyForFalling = -15f;
    public float extraRewardForFinish = 10f;
    public float stepPenalty = -0.01f;

    private Rigidbody rb;
    private List<Transform> checkpoints;
    private Vector3 originalPostion;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        checkpoints = new List<Transform>();
        originalPostion = transform.position;
        // Assuming all checkpoints are tagged "Checkpoint"
        foreach (GameObject checkpointObject in GameObject.FindGameObjectsWithTag("Checkpoint"))
        {
            checkpoints.Add(checkpointObject.transform);
        }

        // Assuming the invisible floor is tagged "InvisFloor"
        groundLayer = LayerMask.GetMask("groundLayer");

        // Assuming the side bounds are tagged "SideBound"
        sideLayer = LayerMask.GetMask("sideLayer");
    }

    public override void OnEpisodeBegin()
    {
        transform.position = originalPostion;
        foreach(Transform checkpoint in checkpoints)
        {
            checkpoint.gameObject.SetActive(true);
        }
        SetReward(0);
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // Distance to the nearest checkpoint
        foreach (Transform checkpoint in checkpoints)
        {
            sensor.AddObservation(Vector3.Distance(transform.position, checkpoint.position));
        }

        // Distance to the nearest side bound
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, 1f, sideLayer))
        {
            sensor.AddObservation(hit.distance);
        }
        else
        {
            sensor.AddObservation(1f); // Assuming the track width is 1 unit
        }
    }
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("groundLayer"))
        {
            // Penalize for falling through the floor
            AddReward(penaltyForFalling);
            EndEpisode();
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Finish")
        {
            
            
                // Extra reward for finishing
            AddReward(extraRewardForFinish);
            EndEpisode();
            
        }
        if (other.gameObject.tag == "Checkpoint")
        {
            AddReward(rewardPerCheckpoint);
            Debug.Log("point gained");
            other.gameObject.SetActive(false);
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("InvisLayer"))
        {
            // Reset the reward if the ball has just exited the floor
            AddReward(-penaltyForFalling);
        }


    }


    public override void OnActionReceived(ActionBuffers actions)
    {
        var moveHorizontal = Mathf.Clamp(actions.ContinuousActions[0], -1f, 1f);
        var moveVertical = Mathf.Clamp(actions.ContinuousActions[1], -1f, 1f);

        // Calculate the desired roll direction based on the input
        Vector3 desiredRollDirection = new Vector3(moveHorizontal, 0, moveVertical).normalized;

        // Apply torque to simulate rolling motion
        rb.AddTorque(desiredRollDirection * 10, ForceMode.VelocityChange);

        AddReward(stepPenalty);
    }


    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActionsOut = actionsOut.ContinuousActions;
        continuousActionsOut[0] = Input.GetAxis("Horizontal");
        continuousActionsOut[1] = Input.GetAxis("Vertical");
    }
}
