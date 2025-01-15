using UnityEngine;

[CreateAssetMenu(fileName = "ShopItem", menuName = "ScriptableObjects/Shop/ShopItem", order = 1)]
public class ShopItemData : ScriptableObject
{
    public string itemName;
    public int startPrice;
    public GameObject itemPrefab;
}
