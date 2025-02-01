using System;
using Managers;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Envinronment.Lights
{
    public class Lamp : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private Sprite _onLightsSprite;
        [SerializeField] private Sprite _offLightsSprite;
        [SerializeField] private Light2D _light2D;

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
            _spriteRenderer.sprite = _onLightsSprite;
            _light2D.enabled = true;
        }
        
        private void TurnLightsOff()
        {
            _spriteRenderer.sprite = _offLightsSprite;
            _light2D.enabled = false;
        }
    }
}
