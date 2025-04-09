using UnityEngine;

namespace ScriptableObjects.Items
{
    /// <summary>
    /// ScriptableObject that defines configuration data for a watering can tool
    /// </summary>
    [CreateAssetMenu(fileName = "WateringCanData", menuName = "ScriptableObjects/WateringCanData")]
    public class WateringCanData : ScriptableObject
    {
        /// <summary>
        /// Affects how far the player can reach when using the watering can
        /// Higher values allow watering plants and refilling at greater distances
        /// </summary>
        public float distanceMultiplier;
        
        /// <summary>
        /// Maximum amount of water that can be stored in the watering can
        /// Represents the can's capacity before needing to be refilled
        /// </summary>
        public int maxWaterAmount;
        
        /// <summary>
        /// Amount of water used/applied per watering action
        /// Affects both how quickly the can empties and how much hydration plants receive
        /// </summary>
        public int wateringAmount;
    }
}