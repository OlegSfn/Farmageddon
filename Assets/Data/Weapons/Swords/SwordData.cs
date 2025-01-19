using UnityEngine;

namespace Data.Weapons.Swords
{
    [CreateAssetMenu(fileName = "SwordData", menuName = "ScriptableObjects/SwordData")]
    public class SwordData : ScriptableObject
    {
        public int damage;
    }
}
