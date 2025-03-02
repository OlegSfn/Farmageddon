using UnityEngine;

namespace ScriptableObjects.Items
{
    [CreateAssetMenu(fileName = "SwordData", menuName = "ScriptableObjects/SwordData")]
    public class SwordData : ScriptableObject
    {
        public int damage;
    }
}
