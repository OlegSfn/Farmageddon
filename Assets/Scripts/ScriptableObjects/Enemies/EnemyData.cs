using UnityEngine;

namespace ScriptableObjects.Enemies
{
    /// <summary>
    /// ScriptableObject that defines configuration data for enemy entities
    /// Contains settings for combat, movement, drops, and targeting priorities
    /// </summary>
    [CreateAssetMenu(fileName = "EnemyData", menuName = "ScriptableObjects/Enemies/Enemy")]
    public class EnemyData : ScriptableObject
    {
        /// <summary>
        /// Base damage dealt by this enemy
        /// </summary>
        public int damage;
        
        /// <summary>
        /// Movement speed of this enemy
        /// </summary>
        public float speed;
        
        /// <summary>
        /// Time in seconds between attacks
        /// </summary>
        public float attackCooldown;
        
        /// <summary>
        /// Item prefab that this enemy drops when killed
        /// </summary>
        public GameObject dropItem;
        
        /// <summary>
        /// Priority settings for which targets to chase
        /// </summary>
        public PriorityMap[] chasingPriorities;
        
        /// <summary>
        /// Priority settings for which targets to attack
        /// </summary>
        public PriorityMap[] attackingPriorities;
        
        /// <summary>
        /// Maps tag names to priority values for enemy targeting
        /// </summary>
        [System.Serializable]
        public struct PriorityMap
        {
            /// <summary>
            /// Unity tag of the potential target
            /// </summary>
            public string colTag;
            
            /// <summary>
            /// Priority value for this target type (higher values = higher priority)
            /// </summary>
            public int priority;
        }
    }
}