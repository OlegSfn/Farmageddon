using TMPro;
using UnityEngine;

/// <summary>
/// Tracks and displays player survival time in MM:SS format
/// </summary>
public class LiveStopwatch : MonoBehaviour
{
    /// <summary>
    /// UI text element to display the stopwatch
    /// </summary>
    [SerializeField] private TextMeshProUGUI stopwatchText;
    
    /// <summary>
    /// Accumulated time since game start
    /// </summary>
    private float _timeLived;
    
    /// <summary>
    /// Updates timer value and UI display every frame
    /// </summary>
    void Update()
    {
        _timeLived += Time.deltaTime;
        
        int minutes = Mathf.FloorToInt(_timeLived / 60f);
        int seconds = Mathf.FloorToInt(_timeLived % 60f);
            
        // Format time as MM:SS
        string formattedTime = $"{minutes:00}:{seconds:00}";
            
        stopwatchText.text = formattedTime;
    }
}