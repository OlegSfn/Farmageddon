using System;
using System.Collections.Generic;
using Managers;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Inventory
{
    /// <summary>
    /// Manages the player's inventory system, including item storage, selection, and manipulation
    /// Handles item stacking, hotkey selection, and interaction with game mechanics
    /// </summary>
    public class Inventory : MonoBehaviour
    {
        /// <summary>
        /// List of inventory slots available to the player
        /// </summary>
        [SerializeField] private List<InventorySlot> inventorySlots;
    
        /// <summary>
        /// Index of the currently selected inventory slot
        /// </summary>
        public int activeItemIndex { get; set; }
    
        /// <summary>
        /// Alpha transparency value used when dragging items
        /// </summary>
        public float dragItemAlpha = 0.6f;
    
        /// <summary>
        /// Tracks the quantity of each type of item in the inventory by name
        /// </summary>
        public readonly Dictionary<string, int> ItemQuantities = new();
        
        /// <summary>
        /// Gets the currently active item from the selected inventory slot
        /// </summary>
        public InventoryItem CurrentActiveItem => inventorySlots[activeItemIndex].item;

        /// <summary>
        /// Initialize inventory slots indices
        /// </summary>
        private void Awake()
        {
            SetupInventorySlotsIndices();
        }

        /// <summary>
        /// Processes input for changing the active inventory slot
        /// Supports both mouse wheel scrolling and number key hotkeys
        /// </summary>
        private void Update()
        {
            HandleMouseScrollSelection();
            HandleNumberKeySelection();
        }

        /// <summary>
        /// Initialize inventory slots indices
        /// </summary>
        private void SetupInventorySlotsIndices()
        {
            for(int i = 0; i < inventorySlots.Count; ++i)
            {
                inventorySlots[i].indexInInventory = i;
            }
        }

        /// <summary>
        /// Handles selection via mouse scroll wheel
        /// </summary>
        private void HandleMouseScrollSelection()
        {
            if (Input.mouseScrollDelta != Vector2.zero)
            {
                int index = activeItemIndex + (int)Input.mouseScrollDelta.y;
                index = Mathf.Clamp(index, 0, inventorySlots.Count - 1);
                ChangeActiveItem(index);
            }
        }
        
        /// <summary>
        /// Handles selection via number keys
        /// </summary>
        private void HandleNumberKeySelection()
        {
            for (int i = 0; i < inventorySlots.Count; ++i)
            {
                if (i == inventorySlots.Count - 1)
                {
                    if (Input.GetKeyDown(KeyCode.Alpha0))
                    {
                        ChangeActiveItem(inventorySlots.Count-1);
                    }
                }
                else
                {
                    if (Input.GetKeyDown(KeyCode.Alpha1 + i))
                    {
                        ChangeActiveItem(i);
                    }
                }
            }
        }

        /// <summary>
        /// Adds items to the inventory, stacking where possible
        /// </summary>
        /// <param name="item">The item to add</param>
        /// <param name="quantity">How many of the item to add</param>
        public void AddItems(InventoryItem item, int quantity)
        {
            int quantityToAdd = TryStackWithExistingItems(item, quantity);
            if (quantityToAdd > 0)
            {
                AddItemToEmptySlot(item, quantityToAdd);
            }
        }
        
        /// <summary>
        /// Attempts to stack an item with existing similar items
        /// </summary>
        /// <param name="item">The item to stack</param>
        /// <param name="quantity">Initial quantity to add</param>
        /// <returns>Remaining quantity that couldn't be stacked</returns>
        private int TryStackWithExistingItems(InventoryItem item, int quantity)
        {
            int quantityToAdd = quantity;
            
            foreach (var inventorySlot in inventorySlots)
            {
                if (inventorySlot.item != null && inventorySlot.item.ItemName == item.ItemName)
                {
                    int quantityToTransfer =
                        Mathf.Min(quantityToAdd, item.MaxStackQuantity - inventorySlot.item.Quantity);
                    
                    inventorySlot.item.Quantity += quantityToTransfer;
                    ChangeItemsCount(item.ItemName, quantityToTransfer);
                    inventorySlot.UpdateUI();
                    quantityToAdd -= quantityToTransfer;
                }

                if (quantityToAdd <= 0)
                {
                    Destroy(item.gameObject);
                    return 0;
                }
            }
            
            return quantityToAdd;
        }
        
        /// <summary>
        /// Adds an item to the first empty inventory slot
        /// </summary>
        /// <param name="item">The item to add</param>
        /// <param name="quantity">Quantity to add</param>
        private void AddItemToEmptySlot(InventoryItem item, int quantity)
        {
            foreach (var inventorySlot in inventorySlots)
            {
                if (inventorySlot.item == null)
                {
                    inventorySlot.item = item;
                    item.transform.position = GameManager.Instance.objectsPool.position;
                    inventorySlot.item.Quantity = quantity;
                    ChangeItemsCount(item.ItemName, inventorySlot.item.Quantity);
                    inventorySlot.item.inventorySlot = inventorySlot;
                    inventorySlot.UpdateUI();
                    if (inventorySlot.indexInInventory == activeItemIndex)
                    {
                        SetItemLogicActive(true);
                    }
                    return;
                }
            }
            
            // If we got here, inventory is full and item couldn't be added
            // The original item still exists in the world
        }

        /// <summary>
        /// Drops an item from the inventory into the game world
        /// </summary>
        /// <param name="item">The item to drop</param>
        public void DropItem(InventoryItem item)
        {
            if (item.inventorySlot == null)
            {
                return;
            }
            
            ChangeItemsCount(item.ItemName, -item.Quantity);
            
            if (item.inventorySlot.indexInInventory == activeItemIndex)
            {
                SetItemLogicActive(false);
            }
            item.inventorySlot.item = null;
            item.inventorySlot.UpdateUI();
            item.inventorySlot = null;
            
            // Drop the item near the player with slight randomization
            Vector2 randomPosition = (Vector2)GameManager.Instance.playerTransform.position + Random.insideUnitCircle * 0.5f;
            item.transform.position = randomPosition;
            item.StartCoroutine(item.HandleDroppingItem());
        }
        
        /// <summary>
        /// Changes which inventory slot is active and updates visuals accordingly
        /// </summary>
        /// <param name="index">Index of the slot to activate</param>
        private void ChangeActiveItem(int index)
        {
            if (activeItemIndex == index)
            {
                return;
            }
        
            inventorySlots[activeItemIndex].slotBorder.enabled = false;
            inventorySlots[index].slotBorder.enabled = true;

            SetItemLogicActive(false);
            activeItemIndex = index;
            SetItemLogicActive(true);
        }
        
        /// <summary>
        /// Activates or deactivates the logic component of the currently selected item
        /// </summary>
        /// <param name="isActive">Whether the item logic should be active</param>
        public void SetItemLogicActive(bool isActive) => SetItemLogicActive(activeItemIndex, isActive);

        /// <summary>
        /// Activates or deactivates the logic component of an item in a specific slot
        /// </summary>
        /// <param name="index">Index of the slot to activate or deactivate item logic</param>
        /// <param name="isActive">Whether the item logic should be active</param>
        public void SetItemLogicActive(int index, bool isActive)
        {
            if (inventorySlots[index].item is not null)
            {
                ILogic logic = inventorySlots[index].item.gameObject.GetComponent<ILogic>();
                if (logic is not null)
                {
                    logic.SetActive(isActive);
                }
            }
        }
        
        /// <summary>
        /// Removes a specific quantity of items by name
        /// Starts removing from the last slots first
        /// </summary>
        /// <param name="itemName">Name of the item to remove</param>
        /// <param name="quantity">Quantity to remove</param>
        public void RemoveItems(string itemName, int quantity)
        {
            int quantityToRemove = quantity;
            
            // Process slots in reverse order (usually consumables are at the end)
            for (int i = inventorySlots.Count-1; i >= 0; --i)
            {
                if (inventorySlots[i].item is not null && itemName == inventorySlots[i].item.ItemName)
                {
                    int itemQuantity = inventorySlots[i].item.Quantity;
                    inventorySlots[i].RemoveFromSlot(Mathf.Min(quantityToRemove, itemQuantity));
                    quantityToRemove -= itemQuantity;
                    
                    if (quantityToRemove <= 0)
                    {
                        break;
                    }
                }
            }
        }
    
        /// <summary>
        /// Updates the tracking dictionary of item quantities
        /// </summary>
        /// <param name="itemName">Name of the item being tracked</param>
        /// <param name="count">Change in quantity (positive for addition, negative for removal)</param>
        public void ChangeItemsCount(string itemName, int count)
        {
            if (count > 0)
            {
                // Adding items
                if (!ItemQuantities.TryAdd(itemName, count))
                {
                    ItemQuantities[itemName] += count;
                }
            }
            else
            {
                // Removing items
                ItemQuantities[itemName] += count;
                if (ItemQuantities[itemName] <= 0)
                {
                    ItemQuantities.Remove(itemName);
                }
            }
        }
    }
}
