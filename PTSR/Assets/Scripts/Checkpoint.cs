using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        RaceAgent agent = other.GetComponent<RaceAgent>();
        if (agent != null)
        {
            agent.CheckpointReached(this);
        }
    }
}
