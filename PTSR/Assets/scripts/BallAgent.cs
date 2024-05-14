using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class BallAgent : Agent
{
    public LayerMask groundLayer;
    public LayerMask sideLayer;
    public float rewardPerCheckpoint = 10f;
    public float penaltyForFalling = -50f;
    public float extraRewardForFinish = 100f;

    private Rigidbody rb;
    private List<Transform> checkpoints;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        checkpoints = new List<Transform>();

        // Assuming all checkpoints are tagged "Checkpoint"
        foreach (GameObject checkpointObject in GameObject.FindGameObjectsWithTag("Checkpoint"))
        {
            checkpoints.Add(checkpointObject.transform);
        }

        // Assuming the invisible floor is tagged "InvisFloor"
        groundLayer = LayerMask.GetMask("InvisFloor");

        // Assuming the side bounds are tagged "SideBound"
        sideLayer = LayerMask.GetMask("SideBound");
    }

    public override void OnEpisodeBegin()
    {
        transform.position = new Vector3(0, 0, 0);
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

    public override void OnActionReceived(ActionBuffers actions)
    {
        var moveHorizontal = Mathf.Clamp(actions.ContinuousActions[0], -1f, 1f);
        var moveVertical = Mathf.Clamp(actions.ContinuousActions[1], -1f, 1f);

        // Apply movement
        rb.AddForce(new Vector3(moveHorizontal, 0, moveVertical));

        // Check for falling through the invisible floor
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, 1f, groundLayer))
        {
            // Penalize for falling through the floor
            AddReward(penaltyForFalling);
            EndEpisode();
        }

        // Check for hitting a checkpoint
        bool hitCheckpoint = false;
        foreach (Transform checkpoint in checkpoints)
        {
            if (Vector3.Distance(transform.position, checkpoint.position) < 1f)
            {
                // Reward for hitting a checkpoint
                AddReward(rewardPerCheckpoint);
                hitCheckpoint = true;
                break;
            }
        }

        // Check for hitting the last checkpoint with the "Finish" tag
        if (hitCheckpoint && gameObject.tag == "Finish")
        {
            // Extra reward for finishing
            AddReward(extraRewardForFinish);
        }

        if (!hitCheckpoint)
        {
            // Continue episode if no checkpoint hit
            EndEpisode();
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActionsOut = actionsOut.ContinuousActions;
        continuousActionsOut[0] = Input.GetAxis("Horizontal");
        continuousActionsOut[1] = Input.GetAxis("Vertical");
    }
}
