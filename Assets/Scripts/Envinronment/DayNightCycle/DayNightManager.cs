using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering.Universal;

namespace Envinronment.DayNightCycle
{
    public class DayNightManager : MonoBehaviour
    {
        [SerializeField] private float dayLength = 60;
        [SerializeField] private float nightLength = 30;
        [SerializeField] private Light2D sun;
        [SerializeField] private Color dayColor;
        [SerializeField] private Color nightColor;
    
        public bool IsDay { get; private set; } = true;
    
        public UnityEvent onDayStart;
        public UnityEvent onNightStart;
    
        private float _currentTime;

        public int DaysNumber { get; private set; } = 1;
    
        private void Update()
        {
            _currentTime += Time.deltaTime;
            if (_currentTime >= dayLength + nightLength)
            {
                _currentTime = 0;
            }
        
            if (_currentTime < dayLength)
            {
                sun.color = Color.Lerp(dayColor, nightColor, _currentTime / dayLength);
                if (!IsDay)
                {
                    onDayStart?.Invoke();
                    IsDay = true;
                    ++DaysNumber;
                }
            }
            else
            {
                sun.color = Color.Lerp(nightColor, dayColor, (_currentTime - dayLength) / nightLength);
                if (IsDay)
                {
                    onNightStart?.Invoke();
                    IsDay = false;
                }
            }
        }
    }
}
