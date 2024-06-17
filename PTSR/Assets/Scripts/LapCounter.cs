using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LapCounter : MonoBehaviour
{
    public TextMeshProUGUI lapCounterText;
    public TextMeshProUGUI lapCounterAi;
    private LapManager lapManager;

    void Start()
    {
        lapManager = FindObjectOfType<LapManager>();
        if (lapManager == null)
        {
            Debug.LogError("LapManager not found in the scene.");
            return;
        }
        UpdateLapCounter();
    }

    void UpdateLapCounter()
    {
        lapCounterAi.text = lapManager.lapCounterAi;
        lapCounterText.text = lapManager.lapCounterText;
    }

    void Update()
    {
        UpdateLapCounter(); // Optional: Call UpdateLapCounter() every frame if you want the UI to update continuously
    }

    public void OnPlayerCompletedLap()
    {
        UpdateLapCounter();
    }
}
