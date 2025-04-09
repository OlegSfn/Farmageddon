using Managers;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Environment.Lights
{
    /// <summary>
    /// Controls a lamp that automatically turns on at night and off during the day
    /// Changes both the visual sprite and light component
    /// </summary>
    public class Lamp : MonoBehaviour
    {
        /// <summary>
        /// Sprite renderer for changing the lamp's appearance
        /// </summary>
        [SerializeField] private SpriteRenderer spriteRenderer;
        
        /// <summary>
        /// Sprite to display when the lamp is lit
        /// </summary>
        [SerializeField] private Sprite onLightsSprite;
        
        /// <summary>
        /// Sprite to display when the lamp is unlit
        /// </summary>
        [SerializeField] private Sprite offLightsSprite;
        
        /// <summary>
        /// Light component that emits light when the lamp is on
        /// </summary>
        [SerializeField] private Light2D light2D;

        /// <summary>
        /// Subscribe to day/night cycle events when enabled
        /// </summary>
        private void OnEnable()
        {
            GameManager.Instance.dayNightManager.onNightStart.AddListener(TurnLightsOn);
            GameManager.Instance.dayNightManager.onDayStart.AddListener(TurnLightsOff);
        }

        /// <summary>
        /// Unsubscribe from day/night cycle events when disabled
        /// </summary>
        private void OnDisable()
        {
            GameManager.Instance.dayNightManager.onNightStart.RemoveListener(TurnLightsOn);
            GameManager.Instance.dayNightManager.onDayStart.RemoveListener(TurnLightsOff);
        }

        /// <summary>
        /// Turns on the lamp - changes sprite and enables light component
        /// Called when night begins
        /// </summary>
        private void TurnLightsOn()
        {
            spriteRenderer.sprite = onLightsSprite;
            light2D.enabled = true;
        }
        
        /// <summary>
        /// Turns off the lamp - changes sprite and disables light component
        /// Called when day begins
        /// </summary>
        private void TurnLightsOff()
        {
            spriteRenderer.sprite = offLightsSprite;
            light2D.enabled = false;
        }
    }
}
