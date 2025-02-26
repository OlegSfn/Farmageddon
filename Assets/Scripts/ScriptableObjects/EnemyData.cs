using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(fileName = "EnemyData", menuName = "ScriptableObjects/Enemies/Enemy")]
    public class EnemyData : ScriptableObject
    {
        public int damage;
        public float speed;
        public float attackCooldown;
        public GameObject dropItem;
    }
}
