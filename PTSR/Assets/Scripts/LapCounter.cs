using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LapCounter : MonoBehaviour
{
    public TextMeshProUGUI lapCounterText;
    private LapManager lapManager;

    void Start()
    {
        lapManager = FindObjectOfType<LapManager>();
        UpdateLapCounter();
    }

    void UpdateLapCounter()
    {
        int currentLap = lapManager.playerLaps["Player"];
        int maxLaps = lapManager.totalLaps;
        lapCounterText.text = currentLap + " / " + maxLaps + " laps";
    }



    void Update()
    {
         UpdateLapCounter();
    }

    public void OnPlayerCompletedLap()
    {
        UpdateLapCounter();
    }
}
