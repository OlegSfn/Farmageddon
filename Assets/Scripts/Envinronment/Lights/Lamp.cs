using Managers;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Envinronment.Lights
{
    public class Lamp : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private Sprite onLightsSprite;
        [SerializeField] private Sprite offLightsSprite;
        [SerializeField] private Light2D light2D;

        private void OnEnable()
        {
            GameManager.Instance.dayNightManager.onNightStart.AddListener(TurnLightsOn);
            GameManager.Instance.dayNightManager.onDayStart.AddListener(TurnLightsOff);
        }

        private void OnDisable()
        {
            GameManager.Instance.dayNightManager.onNightStart.RemoveListener(TurnLightsOn);
            GameManager.Instance.dayNightManager.onDayStart.RemoveListener(TurnLightsOff);
        }


        private void TurnLightsOn()
        {
            spriteRenderer.sprite = onLightsSprite;
            light2D.enabled = true;
        }
        
        private void TurnLightsOff()
        {
            spriteRenderer.sprite = offLightsSprite;
            light2D.enabled = false;
        }
    }
}
