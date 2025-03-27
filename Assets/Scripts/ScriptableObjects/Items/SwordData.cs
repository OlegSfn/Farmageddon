using UnityEngine;

namespace ScriptableObjects.Items
{
    /// <summary>
    /// ScriptableObject that defines configuration data for a sword weapon
    /// </summary>
    [CreateAssetMenu(fileName = "SwordData", menuName = "ScriptableObjects/SwordData")]
    public class SwordData : ScriptableObject
    {
        /// <summary>
        /// Base damage dealt by this sword
        /// </summary>
        public int damage;
    }
}