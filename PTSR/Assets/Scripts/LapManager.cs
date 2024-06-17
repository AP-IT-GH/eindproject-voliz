using UnityEngine;
using UnityEngine.SceneManagement;

public class LapManager : MonoBehaviour
{
    public int totalLaps = 3; // Total laps required to finish the race
    private int lapCount = 0; // Current lap count
    private int lapCountAi = 0; 
    public string lapCounterText = "Lap: 0" ;
    public string lapCounterAi = "Lap: 0";

    public void Start()
    {
        lapCounterText = "Lap: " + lapCount;
        lapCounterAi = "Ai Lap: " + lapCountAi;
    }
    public void IncrementLap()
    {
        lapCount++;
        lapCounterText = "Lap: " + lapCount;
        Debug.Log("Current Lap: " + lapCount);
        if (lapCount == totalLaps)
        {
            
            LoadFinishScene();
        }
    }
    public void IncrementLapAi()
    {
        lapCountAi++;
        lapCounterAi = "Ai Lap: " + lapCountAi;
        Debug.Log("Current Lap: " + lapCountAi);
        if (lapCountAi == totalLaps)
        {

            LoadLoseScene();
        }
    }

    public int GetCurrentLap()
    {
        return lapCount;
    }
    private void LoadFinishScene()
    {
        SceneManager.LoadScene("Finish"); // Replace "Finish" with the exact name of your scene
    }
    private void LoadLoseScene()
    {
        SceneManager.LoadScene("Lose"); // Replace "Finish" with the exact name of your scene
    }
}
