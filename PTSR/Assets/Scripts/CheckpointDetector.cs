using System.Collections;
using UnityEngine;

public class CheckpointDetector : MonoBehaviour
{
    public LapManager lapManager;
    private bool isCooldownActive = false; // Track if the cooldown is active
    private bool isCooldownActiveAi = false; // Track if the cooldown is active

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // Ensure your player GameObject has a tag named "Player"
        {
            if (!isCooldownActive)
            {
                StartCoroutine(Cooldown());
                lapManager.IncrementLap();
            }
        }
        if (other.CompareTag("AiPlayer")) // Ensure your player GameObject has a tag named "Player"
        {
            if (!isCooldownActiveAi)
            {
                StartCoroutine(CooldownAi());
                lapManager.IncrementLapAi();
            }
        }
    }

    IEnumerator Cooldown()
    {
        isCooldownActive = true; // Activate cooldown
        yield return new WaitForSeconds(10); // Wait for 10 seconds
        isCooldownActive = false; // Deactivate cooldown after waiting
    }
    IEnumerator CooldownAi()
    {
        isCooldownActiveAi = true; // Activate cooldown
        yield return new WaitForSeconds(10); // Wait for 10 seconds
        isCooldownActiveAi = false; // Deactivate cooldown after waiting
    }
}
