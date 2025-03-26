using ScriptableObjects.Shop;
using UnityEngine;

namespace ScriptableObjects.Quests
{
    [CreateAssetMenu(fileName = "Quest", menuName = "ScriptableObjects/Quests/Quest", order = 1)]
    public class QuestData : ScriptableObject
    {
        public ShopItemData itemToSell;
        public Sprite itemSprite;
        public int requiredAmount;

        public int daysLimit;
        public int rewardAmount;
    }
}