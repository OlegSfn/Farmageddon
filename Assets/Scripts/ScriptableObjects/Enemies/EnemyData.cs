using UnityEngine;

namespace ScriptableObjects.Enemies
{
    [CreateAssetMenu(fileName = "EnemyData", menuName = "ScriptableObjects/Enemies/Enemy")]
    public class EnemyData : ScriptableObject
    {
        public int damage;
        public float speed;
        public float attackCooldown;
        public GameObject dropItem;
        
        public PriorityMap[] chasingPriorities;
        public PriorityMap[] attackingPriorities;
        
        [System.Serializable]
        public struct PriorityMap
        {
            public string colTag;
            public int priority;
        }
    }
}
