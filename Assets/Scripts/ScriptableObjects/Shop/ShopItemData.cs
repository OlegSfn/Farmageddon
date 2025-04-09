using UnityEngine;

namespace ScriptableObjects.Shop
{
    /// <summary>
    /// ScriptableObject that defines data for an item that can be bought or sold in shops
    /// </summary>
    [CreateAssetMenu(fileName = "ShopItem", menuName = "ScriptableObjects/Shop/ShopItem", order = 1)]
    public class ShopItemData : ScriptableObject
    {
        /// <summary>
        /// Display name of the item shown in shop UI
        /// </summary>
        public string itemName;
        
        /// <summary>
        /// Base price to buy this item from a shop
        /// </summary>
        public int startBuyPrice;
        
        /// <summary>
        /// Base price received when selling this item to a shop
        /// </summary>
        public int startSellPrice;
        
        /// <summary>
        /// The actual game object created when this item is bought or dropped in the world
        /// </summary>
        public GameObject itemPrefab;
    }
}