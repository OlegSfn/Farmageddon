using UnityEngine;
using UnityEngine.Events;

namespace Helpers
{
    /// <summary>
    /// Manages entity health, damage, and healing
    /// Provides events for health changes and death
    /// </summary>
    public class HealthController : MonoBehaviour
    {
        /// <summary>
        /// Maximum health capacity of the entity
        /// </summary>
        [field: SerializeField] public int MaxHealth { get; private set; }
    
        /// <summary>
        /// Event triggered when health value changes
        /// Provides hit information (can be null for healing) and new health value
        /// </summary>
        [SerializeField] private UnityEvent<HitInfo?, int> onValueChange;
    
        /// <summary>
        /// Event triggered when health reaches zero or below
        /// </summary>
        [SerializeField] private UnityEvent onDeath;

        /// <summary>
        /// Current health value
        /// </summary>
        private int _health;
    
        /// <summary>
        /// Initialize health to maximum on component awake
        /// </summary>
        private void Awake()
        {
            _health = MaxHealth;
        }
    
        /// <summary>
        /// Apply damage to the entity
        /// </summary>
        /// <param name="hitInfo">Information about the hit causing damage</param>
        public void TakeDamage(HitInfo hitInfo)
        {
            _health -= hitInfo.Damage;
            onValueChange?.Invoke(hitInfo, _health);
        
            // Check for death condition
            if (_health <= 0)
            {
                onDeath?.Invoke();
            }
        }

        /// <summary>
        /// Restore health to the entity
        /// </summary>
        /// <param name="amount">Amount of health to restore</param>
        public void Heal(int amount)
        {
            _health += amount;
        
            if (_health > MaxHealth)
            {
                _health = MaxHealth;
            }
        
            onValueChange?.Invoke(null, _health);
        }
    }
}
