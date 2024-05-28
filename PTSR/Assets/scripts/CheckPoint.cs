using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public int checkpointIndex; // The index of this checkpoint

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Player2"))
        {
            LapManager lapManager = FindObjectOfType<LapManager>();
            lapManager.PlayerPassedCheckpoint(other.tag, checkpointIndex);
        }
    }
}
