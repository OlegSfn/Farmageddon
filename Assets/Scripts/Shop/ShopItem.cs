using System.Collections.Generic;
using System.Text;
using Inventory;
using Managers;
using ScriptableObjects.Shop;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Shop
{
    /// <summary>
    /// Represents an individual item in the shop interface
    /// Handles buying/selling logic, price fluctuations, and UI updates
    /// </summary>
    public class ShopItem : MonoBehaviour, IScrollHandler, IPointerEnterHandler, IPointerExitHandler
    {
        /// <summary>
        /// Whether this item slot is for selling or buying
        /// </summary>
        [field: SerializeField] public bool isSelling { get; private set; }
        
        /// <summary>
        /// Reference to the parent shop component
        /// </summary>
        [SerializeField] private Shop shop;
        
        /// <summary>
        /// Data configuration for this shop item
        /// </summary>
        [SerializeField] private ShopItemData itemData;

        /// <summary>
        /// Button that initiates the buy or sell transaction
        /// </summary>
        [SerializeField] private Button buySellButton;
        
        /// <summary>
        /// Text component for displaying the item name
        /// </summary>
        [SerializeField] private TextMeshProUGUI itemName;
        
        /// <summary>
        /// Text component for displaying the current price
        /// </summary>
        [SerializeField] private TextMeshProUGUI priceText;
        
        /// <summary>
        /// Text component for displaying quantity information
        /// </summary>
        [SerializeField] private TextMeshProUGUI itemCountText;
    
        /// <summary>
        /// Current price of the item
        /// </summary>
        private int _price;
        
        /// <summary>
        /// Base price of the item (before fluctuations)
        /// </summary>
        private int _startPrice;
        
        /// <summary>
        /// Currently selected quantity to buy/sell
        /// </summary>
        private int _itemCount;
        
        /// <summary>
        /// Maximum available quantity based on inventory or cash
        /// </summary>
        private int _maxItemCount;
        
        /// <summary>
        /// Minimum multiplier for price decreases (daily fluctuations)
        /// </summary>
        private readonly float _minDecreasePriceMultiplier = 0.8f;
        
        /// <summary>
        /// Maximum multiplier for price increases (daily fluctuations)
        /// </summary>
        private readonly float _maxIncreasePriceMultiplier = 1.2f;
        
        /// <summary>
        /// Consecutive days this item has been sold, affects price
        /// </summary>
        private int _sellRow;
        
        /// <summary>
        /// Whether this item was sold on the previous day
        /// </summary>
        private bool _wasSoldLastDay;
        
        /// <summary>
        /// Whether the mouse is currently hovering over this item
        /// </summary>
        private bool _isMouseOver;
        
        /// <summary>
        /// Cooldown between item count changes via keyboard
        /// </summary>
        private readonly float _itemCountChangeInterval = 0.1f;
        
        /// <summary>
        /// Timer for item count change cooldown
        /// </summary>
        private float _itemCountChangeTimer;

        /// <summary>
        /// Initializes the shop item with its starting values
        /// </summary>
        private void Awake()
        {
            _startPrice = isSelling ? itemData.startSellPrice : itemData.startBuyPrice;
            _price = _startPrice;
            
            GameManager.Instance.dayNightManager.onDayStart.AddListener(OnDayChange);
            
            // Will not change later.
            itemName.text = GetSpriteAssetText(itemData.itemName);
            UpdateUI();
        }

        /// <summary>
        /// Handles keyboard input for changing item quantities
        /// </summary>
        private void Update()
        {
            if (!_isMouseOver || _itemCountChangeTimer > 0)
            {
                if (_itemCountChangeTimer > 0)
                {
                    _itemCountChangeTimer -= Time.deltaTime;
                }
                return;
            }
            
            HandleKeyboardInput();
        }
        
        /// <summary>
        /// Processes keyboard input for changing item quantity
        /// </summary>
        private void HandleKeyboardInput()
        {
            if (Input.GetAxisRaw("Vertical") > 0)
            {
                ++_itemCount;
                _itemCountChangeTimer = _itemCountChangeInterval;
            }
            else if (Input.GetAxisRaw("Vertical") < 0)
            {
                --_itemCount;
                _itemCountChangeTimer = _itemCountChangeInterval;
            }
            
            _itemCount = Mathf.Clamp(_itemCount, 0, _maxItemCount);
            
            UpdateUI();
        }
    
        /// <summary>
        /// Updates all UI elements for this shop item
        /// </summary>
        public void UpdateUI()
        {
            UpdateData();
            priceText.text = $"{_price}$";
            itemCountText.text = $"{_itemCount}/{_maxItemCount}";
            buySellButton.interactable = _itemCount > 0;
        }

        /// <summary>
        /// Updates the internal data based on player inventory and cash
        /// </summary>
        private void UpdateData()
        {
            if (isSelling)
            {
                UpdateSellingData();
            }
            else
            {
                UpdateBuyingData();
            }

            _itemCount = Mathf.Min(_itemCount, _maxItemCount);
        }
        
        /// <summary>
        /// Updates data specific to selling items
        /// </summary>
        private void UpdateSellingData()
        {
            _maxItemCount = GameManager.Instance.inventory.ItemQuantities.GetValueOrDefault(itemData.itemName, 0);
        }
        
        /// <summary>
        /// Updates data specific to buying items
        /// </summary>
        private void UpdateBuyingData()
        {
            _price = _price == 0 ? itemData.startBuyPrice : _price;
            
            _maxItemCount = GameManager.Instance.cashManager.Cash / _price;
        }

        /// <summary>
        /// Button handler for buy/sell actions
        /// </summary>
        public void ShopButtonAction()
        {
            if (isSelling)
            {
                Sell();
            }
            else
            {
                Buy();
            }
        }
        
        /// <summary>
        /// Handles buying items from the shop
        /// Creates the purchased items near the player
        /// </summary>
        private void Buy()
        {
            if (GameManager.Instance.cashManager.Cash < _price * _itemCount)
            {
                return;
            }
            
            GameManager.Instance.cashManager.Cash -= _price * _itemCount;
            SpawnPurchasedItems();
            shop.UpdateUI();
        }
        
        /// <summary>
        /// Creates the purchased item instances in the game world
        /// </summary>
        private void SpawnPurchasedItems()
        {
            int quantityToAdd = _itemCount;
            while(quantityToAdd > 0)
            {
                Vector2 randomPosition = (Vector2)GameManager.Instance.playerTransform.position + Random.insideUnitCircle * 0.5f;
                InventoryItem newItem = Instantiate(itemData.itemPrefab, randomPosition, Quaternion.identity).GetComponent<InventoryItem>();
                
                newItem.Quantity = Mathf.Min(quantityToAdd, newItem.MaxStackQuantity);
                quantityToAdd -= newItem.Quantity;
            }
        }

        /// <summary>
        /// Handles selling items to the shop
        /// </summary>
        private void Sell()
        {
            GameManager gameManager = GameManager.Instance;
            
            gameManager.cashManager.Cash += _price * _itemCount;
            gameManager.inventory.RemoveItems(itemData.itemName, _itemCount);
            
            _wasSoldLastDay = true;
            gameManager.questManager.CheckQuestProgress(itemData.itemName, _itemCount);
            shop.UpdateUI();
        }

        /// <summary>
        /// Handles mouse wheel scrolling to change item quantity
        /// </summary>
        public void OnScroll(PointerEventData eventData)
        {
            if (eventData.scrollDelta.y > 0)
            {
                ++_itemCount;
            }
            else
            {
                --_itemCount;
            }

            _itemCount = Mathf.Clamp(_itemCount, 0, _maxItemCount);

            UpdateUI();
        }

        /// <summary>
        /// Updates prices at the start of each new day
        /// Implements the dynamic economy system with price fluctuations
        /// </summary>
        private void OnDayChange()
        {
            UpdatePriceForNewDay();
            UpdateUI();
        }
        
        /// <summary>
        /// Calculates new prices based on market demand simulation
        /// </summary>
        private void UpdatePriceForNewDay()
        {
            float randomPriceMultiplier = Random.Range(_minDecreasePriceMultiplier, _maxIncreasePriceMultiplier);
            _sellRow = _wasSoldLastDay ? _sellRow + 1 : 0;
            if (!_wasSoldLastDay && _price == 0)
            {
                _price += _startPrice / 10 == 0 ? 1 : _startPrice / 10;
            }
            else
            {
                _price = Mathf.Max(0, (int)(_price * randomPriceMultiplier * (1 - _sellRow / 10f)));
            }

            _wasSoldLastDay = false;
        }

        /// <summary>
        /// Converts regular text to TextMeshPro sprite asset text
        /// This allows displaying special character sprites for item names
        /// </summary>
        /// <param name="text">The input text to convert</param>
        /// <returns>Formatted sprite asset text</returns>
        private string GetSpriteAssetText(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return string.Empty;
            }

            StringBuilder result = new StringBuilder();
            foreach (char c in text)
            {
                if (c == ' ')
                {
                    result.Append(' ');
                }
                else
                {
                    result.Append("<sprite name=").Append(c).Append(">");
                }
            }
    
            return result.ToString();
        }

        /// <summary>
        /// Unsubscribes from events when this object is destroyed
        /// </summary>
        private void OnDestroy()
        {
            GameManager.Instance.dayNightManager.onDayStart.RemoveListener(OnDayChange);
        }

        /// <summary>
        /// Tracks when the pointer enters this item's UI area
        /// </summary>
        public void OnPointerEnter(PointerEventData _)
        {
            _isMouseOver = true;
        }

        /// <summary>
        /// Tracks when the pointer exits this item's UI area
        /// </summary>
        public void OnPointerExit(PointerEventData _)
        {
            _isMouseOver = false;
        }

        /// <summary>
        /// Resets state when this component is disabled
        /// </summary>
        public void OnDisable()
        {
            _isMouseOver = false;
            _itemCount = 0;
        }
    }
}
