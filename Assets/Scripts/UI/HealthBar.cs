using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    /// <summary>
    /// Manages the visual representation of player health using a slider and gradient color
    /// </summary>
    public class HealthBar : MonoBehaviour
    {
        /// <summary>
        /// Reference to the player's health system
        /// </summary>
        [SerializeField] private HealthController playerHealthController;
        
        /// <summary>
        /// Slider component representing health percentage
        /// </summary>
        [SerializeField] private Slider slider;
        
        /// <summary>
        /// Image component that changes color based on health
        /// </summary>
        [SerializeField] private Image fillImage;
        
        /// <summary>
        /// Color gradient for health visualization (from full to empty)
        /// </summary>
        [SerializeField] private Gradient gradient;

        /// <summary>
        /// Initializes health bar with max health value
        /// </summary>
        private void Start()
        {
            slider.maxValue = playerHealthController.MaxHealth;
            SetHealth(playerHealthController.MaxHealth);
        }

        /// <summary>
        /// Updates health display when damage is taken
        /// </summary>
        /// <param name="_">Unused hit information (required by event signature)</param>
        /// <param name="remainingHealth">Current health value to display</param>
        public void ChangeHp(HitInfo? _, int remainingHealth)
        {
            SetHealth(remainingHealth);
        }
        
        /// <summary>
        /// Sets health bar value and color
        /// </summary>
        /// <param name="health">Current health value</param>
        public void SetHealth(int health)
        {
            slider.value = health;
            fillImage.color = gradient.Evaluate(slider.normalizedValue);
        }
    }
}