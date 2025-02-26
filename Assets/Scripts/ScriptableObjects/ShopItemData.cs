using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(fileName = "ShopItem", menuName = "ScriptableObjects/Shop/ShopItem", order = 1)]
    public class ShopItemData : ScriptableObject
    {
        public string itemName;
        public int startBuyPrice;
        public int startSellPrice;
        public GameObject itemPrefab;
    }
}
