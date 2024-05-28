using UnityEngine;
using System.Collections.Generic;

public class LapManager : MonoBehaviour
{
    public int totalLaps = 3; // Total laps required to finish the race
    private int totalCheckpoints = 2; // Number of checkpoints

    public Dictionary<string, int> playerLaps = new Dictionary<string, int>()
    {
        {"Player", 0},
        {"Player2", 0}
    };
    private Dictionary<string, List<int>> playerCheckpoints = new Dictionary<string, List<int>>();
    private bool raceFinished = false;

    void Start()
    {
        playerLaps["Player"] = 0;
        playerLaps["Player2"] = 0;
        playerCheckpoints["Player"] = new List<int>();
        playerCheckpoints["Player2"] = new List<int>();
    }

    public void PlayerPassedCheckpoint(string playerTag, int checkpointIndex)
    {
        if (raceFinished) return;

        // Ensure the checkpoints are passed in order
        if (!playerCheckpoints[playerTag].Contains(checkpointIndex) &&
            (playerCheckpoints[playerTag].Count == 0 || playerCheckpoints[playerTag].Count == checkpointIndex))
        {
            playerCheckpoints[playerTag].Add(checkpointIndex);
        }
    }

    public bool PlayerHasPassedAllCheckpoints(string playerTag)
    {
        // Ensure the playerTag is valid
        if (!playerCheckpoints.ContainsKey(playerTag))
        {
            Debug.LogError("Player tag " + playerTag + " not found in dictionary.");
            return false;
        }

        return playerCheckpoints[playerTag].Count == totalCheckpoints;
    }

    public void PlayerCompletedLap(string playerTag)
    {
        playerLaps[playerTag]++;
        Debug.Log(playerTag + " completed lap " + playerLaps[playerTag]);

        if (playerLaps[playerTag] >= totalLaps)
        {
            raceFinished = true;
            Debug.Log(playerTag + " Wins!");
            // Add your logic here to handle end of race (e.g., show win screen)
        }

        // Clear checkpoints for the player after completing the lap
        playerCheckpoints[playerTag].Clear();
    }
}
