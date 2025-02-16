using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(fileName = "SwordData", menuName = "ScriptableObjects/SwordData")]
    public class SwordData : ScriptableObject
    {
        public int damage;
    }
}
