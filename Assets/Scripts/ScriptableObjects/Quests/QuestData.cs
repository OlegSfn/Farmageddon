using ScriptableObjects.Shop;
using UnityEngine;

namespace ScriptableObjects.Quests
{
    /// <summary>
    /// ScriptableObject that defines a quest configuration
    /// </summary>
    [CreateAssetMenu(fileName = "Quest", menuName = "ScriptableObjects/Quests/Quest", order = 1)]
    public class QuestData : ScriptableObject
    {
        /// <summary>
        /// The item that needs to be collected or produced for this quest
        /// </summary>
        public ShopItemData itemToSell;
        
        /// <summary>
        /// Visual representation of the quest item for UI display
        /// </summary>
        public Sprite itemSprite;
        
        /// <summary>
        /// Number of items needed to complete the quest
        /// </summary>
        public int requiredAmount;

        /// <summary>
        /// Time limit in game days to complete the quest
        /// </summary>
        public int daysLimit;
        
        /// <summary>
        /// Cash reward given to the player upon quest completion
        /// </summary>
        public int rewardAmount;
    }
}