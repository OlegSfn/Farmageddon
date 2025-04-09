using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering.Universal;

namespace Environment.DayNightCycle
{
    /// <summary>
    /// Manages the day-night cycle in the game
    /// Controls sun color, time transitions, and triggers day/night events
    /// </summary>
    public class DayNightManager : MonoBehaviour
    {
        /// <summary>
        /// Duration of daytime in seconds
        /// </summary>
        [SerializeField] private float dayLength = 60;
        
        /// <summary>
        /// Duration of nighttime in seconds
        /// </summary>
        [SerializeField] private float nightLength = 30;
        
        /// <summary>
        /// Reference to the main light that represents the sun
        /// </summary>
        [SerializeField] private Light2D sun;
        
        /// <summary>
        /// Color of the light during daytime
        /// </summary>
        [SerializeField] private Color dayColor;
        
        /// <summary>
        /// Color of the light during nighttime
        /// </summary>
        [SerializeField] private Color nightColor;
    
        /// <summary>
        /// Indicates whether it's currently day or night
        /// </summary>
        public bool IsDay { get; private set; } = true;
    
        /// <summary>
        /// Event triggered when day or night begins
        /// </summary>
        public UnityEvent onDayStart;
        public UnityEvent onNightStart;
    
        /// <summary>
        /// Current time within the day-night cycle
        /// </summary>
        private float _currentTime;

        /// <summary>
        /// Current day number since the game started
        /// </summary>
        private int DaysNumber { get; set; } = 1;
    
        /// <summary>
        /// Updates the day-night cycle every frame
        /// Manages time progression, light color transitions, and day/night state changes
        /// </summary>
        private void Update()
        {
            _currentTime += Time.deltaTime;
            if (_currentTime >= dayLength + nightLength)
            {
                _currentTime = 0;
            }
        
            if (_currentTime < dayLength)
            {
                // During day: gradually transition light color from day to night.
                sun.color = Color.Lerp(dayColor, nightColor, _currentTime / dayLength);
                
                // If we already transitioned to day, do nothing.
                if (IsDay)
                {
                    return;
                }
                
                onDayStart?.Invoke();
                IsDay = true;
                ++DaysNumber;
            }
            else
            {
                // During night: gradually transition light color from night to day.
                sun.color = Color.Lerp(nightColor, dayColor, (_currentTime - dayLength) / nightLength);
                
                // If we already transitioned to night, do nothing.
                if (!IsDay)
                {
                    return;
                }
                
                onNightStart?.Invoke();
                IsDay = false;
            }
        }
    }
}
