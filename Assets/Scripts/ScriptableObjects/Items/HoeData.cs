using UnityEngine;

namespace ScriptableObjects.Items
{
    /// <summary>
    /// ScriptableObject that defines configuration data for a hoe tool
    /// </summary>
    [CreateAssetMenu(fileName = "HoeData", menuName = "ScriptableObjects/HoeData")]
    public class HoeData : ScriptableObject
    {
        /// <summary>
        /// Affects how far the player can reach when using the hoe
        /// Higher values allow weeding at greater distances
        /// </summary>
        public float distanceMultiplier;
    }
}