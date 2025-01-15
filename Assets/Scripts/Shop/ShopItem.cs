using System.Collections.Generic;
using Inventory;
using Managers;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Shop
{
    public class ShopItem : MonoBehaviour, IScrollHandler
    {
        [SerializeField] private Shop shop;
        [SerializeField] private ShopItemData itemData;

        [SerializeField] private bool isSelling;
        [SerializeField] private Button buySellButton;
        [SerializeField] private TextMeshProUGUI priceText;
        [SerializeField] private TextMeshProUGUI itemCountText;
    
        private int _price;
        private int _itemCount;
        private int _maxItemCount;
    
        private void Awake()
        {
            _price = itemData.startPrice;
            UpdateUI();
        }
    
        public void UpdateUI()
        {
            UpdateData();
            priceText.text = $"{_price}$";
            itemCountText.text = $"{_itemCount}/{_maxItemCount}";
            buySellButton.interactable = _itemCount > 0;
        }

        private void UpdateData()
        {
            if (_price == 0)
            {
                _price = itemData.startPrice;
            }
        
            if (isSelling)
            {
                _maxItemCount = GameManager.Instance.inventory.ItemQuantities.GetValueOrDefault(itemData.itemName, 0);
            }
            else
            {
                _maxItemCount = GameManager.Instance.money / _price;
            }

            _itemCount = Mathf.Min(_itemCount, _maxItemCount);
        }


        public void Buy()
        {
            if (GameManager.Instance.money < _price * _itemCount) return;
        
            GameManager gameManager = GameManager.Instance;
            InventoryItem item = Instantiate(itemData.itemPrefab, gameManager.objectsPool.position, Quaternion.identity).GetComponent<InventoryItem>();
            GameManager.Instance.money -= _price * _itemCount;
            GameManager.Instance.inventory.AddItems(item, _itemCount);
            shop.UpdateUI();
        }

        public void Sell()
        {
            GameManager gameManager = GameManager.Instance;
            gameManager.money += _price * _itemCount;
            gameManager.inventory.RemoveItems(itemData.itemName, _itemCount);
            shop.UpdateUI();
        }

        public void OnScroll(PointerEventData eventData)
        {
            if (eventData.scrollDelta.y > 0)
            {
                _itemCount++;
            }
            else
            {
                _itemCount--;
            }

            if (_itemCount < 0)
            {
                _itemCount = 0;
            }
            else if (_itemCount > _maxItemCount)
            {
                _itemCount = _maxItemCount;
            }
        
            UpdateUI();
        }
    }
}
