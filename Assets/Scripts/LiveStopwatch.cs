using TMPro;
using UnityEngine;

public class LiveStopwatch : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI stopwatchText;
    
    private float _timeLived;
    
    void Update()
    {
        _timeLived += Time.deltaTime;
        
        int minutes = Mathf.FloorToInt(_timeLived / 60f);
        int seconds = Mathf.FloorToInt(_timeLived % 60f);
            
        string formattedTime = $"{minutes:00}:{seconds:00}";
            
        stopwatchText.text = formattedTime;
    }
}
