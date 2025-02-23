using System;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class HealthBar : MonoBehaviour
    {
        [SerializeField] private HealthController playerHealthController;
        [SerializeField] private Slider slider;
        [SerializeField] private Image fillImage;
        [SerializeField] private Gradient gradient;

        private void Start()
        {
            slider.maxValue = playerHealthController.MaxHealth;
            SetHealth(playerHealthController.MaxHealth);
        }

        public void TakeDamage(HitInfo hitInfo, int remainingHealth)
        {
            SetHealth(remainingHealth);
        }
        
        public void SetHealth(int health)
        {
            slider.value = health;
            fillImage.color = gradient.Evaluate(slider.normalizedValue);
        }
    }
}
