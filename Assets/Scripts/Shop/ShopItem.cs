using System.Collections.Generic;
using Inventory;
using Managers;
using ScriptableObjects;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Shop
{
    public class ShopItem : MonoBehaviour, IScrollHandler
    {
        [SerializeField] private Shop shop;
        [SerializeField] private ShopItemData itemData;

        [SerializeField] private bool isSelling;
        [SerializeField] private Button buySellButton;
        [SerializeField] private TextMeshProUGUI itemName;
        [SerializeField] private TextMeshProUGUI priceText;
        [SerializeField] private TextMeshProUGUI itemCountText;
    
        private int _price;
        private int _startPrice;
        private int _itemCount;
        private int _maxItemCount;
        
        private readonly float _minDecreasePriceMultiplier = 0.8f;
        private readonly float _maxIncreasePriceMultiplier = 1.2f;
        private int _sellRow;
        private bool _wasSelledLastDay;

        private void Awake()
        {
            _startPrice = isSelling ? itemData.startSellPrice : itemData.startBuyPrice;
            _price = _startPrice;
            GameManager.Instance.dayNightManager.onDayStart.AddListener(OnDayChange);
            itemName.text = itemData.itemName;
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
            if (isSelling)
            {
                _maxItemCount = GameManager.Instance.inventory.ItemQuantities.GetValueOrDefault(itemData.itemName, 0);
            }
            else
            {
                _price = _price == 0 ? itemData.startBuyPrice : _price;
                _maxItemCount = GameManager.Instance.cashManager.Cash / _price;
            }

            _itemCount = Mathf.Min(_itemCount, _maxItemCount);
        }
        
        public void Buy()
        {
            if (GameManager.Instance.cashManager.Cash < _price * _itemCount)
            {
                return;
            }
            
            GameManager.Instance.cashManager.Cash -= _price * _itemCount;
            int quantityToAdd = _itemCount;
            while(quantityToAdd > 0)
            {
                Vector2 randomPosition = (Vector2)GameManager.Instance.playerTransform.position + Random.insideUnitCircle * 0.5f;
                InventoryItem newItem = Instantiate(itemData.itemPrefab, randomPosition, Quaternion.identity).GetComponent<InventoryItem>();
                newItem.Quantity = Mathf.Min(quantityToAdd, newItem.MaxStackQuantity);
                quantityToAdd -= newItem.Quantity;
            }
            
            shop.UpdateUI();
        }

        public void Sell()
        {
            GameManager gameManager = GameManager.Instance;
            GameManager.Instance.cashManager.Cash += _price * _itemCount;
            gameManager.inventory.RemoveItems(itemData.itemName, _itemCount);
            _wasSelledLastDay = true;
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

        private void OnDayChange()
        {
            float randomPriceMultiplier = Random.Range(_minDecreasePriceMultiplier, _maxIncreasePriceMultiplier);
            _sellRow = _wasSelledLastDay ? _sellRow + 1 : 0;
            if (!_wasSelledLastDay && _price == 0)
            {
                _price += _startPrice / 10 == 0 ? 1 : _startPrice / 10;
            }
            else
            {
                _price = Mathf.Max(0, (int)(_price * randomPriceMultiplier * (1 - _sellRow/10f)));
            }
            UpdateUI();
        }

        private void OnDestroy()
        {
            GameManager.Instance.dayNightManager.onDayStart.RemoveListener(OnDayChange);
        }
    }
}
