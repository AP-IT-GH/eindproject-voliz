using UnityEngine;

public class FinishLine : MonoBehaviour
{
    private LapManager lapManager;

    void Start()
    {
        lapManager = FindObjectOfType<LapManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Player2"))
        {
            // Only count the lap if the player has passed all checkpoints
            if (lapManager.PlayerHasPassedAllCheckpoints(other.tag))
            {
                lapManager.PlayerCompletedLap(other.tag);
            }
        }
    }
}
