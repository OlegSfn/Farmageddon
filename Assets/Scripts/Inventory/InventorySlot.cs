using Managers;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Inventory
{
    /// <summary>
    /// Represents a single slot in the player's inventory
    /// Handles UI representation and drag/drop interactions for items
    /// </summary>
    public class InventorySlot : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        /// <summary>
        /// Reference to the parent inventory
        /// </summary>
        [SerializeField] private Inventory inventory;
        
        /// <summary>
        /// Image component that displays the item icon
        /// </summary>
        [SerializeField] private Image itemIcon;
        
        /// <summary>
        /// Text component that displays the item quantity
        /// </summary>
        [SerializeField] private TextMeshProUGUI itemQuantityText;
        
        /// <summary>
        /// Canvas group used to control opacity during drag operations
        /// </summary>
        [SerializeField] private CanvasGroup canvasGroup;

        /// <summary>
        /// Border image that highlights the selected slot
        /// </summary>
        [field: SerializeField] public Image slotBorder { get; set; }
        
        /// <summary>
        /// The inventory item contained in this slot, or null if empty
        /// </summary>
        public InventoryItem item { get; set; }

        public int indexInInventory { get; set; }
        
        /// <summary>
        /// Initialize the slot UI on awake
        /// </summary>
        private void Awake()
        {
            UpdateUI();
        }

        /// <summary>
        /// Updates the visual representation of the slot based on its current item
        /// Shows/hides icon and quantity text as appropriate
        /// </summary>
        public void UpdateUI()
        {
            if (item == null)
            {
                ShowEmptySlot();
                return;
            }
        
            ShowItemInSlot();
        }

        /// <summary>
        /// Shows the slot as empty (no icon, no quantity)
        /// </summary>
        private void ShowEmptySlot()
        {
            itemIcon.enabled = false;
            itemQuantityText.enabled = false;
        }

        /// <summary>
        /// Shows the item icon and quantity (if stackable)
        /// </summary>
        private void ShowItemInSlot()
        {
            itemIcon.enabled = true;
            itemIcon.sprite = item.Icon;
            
            // Only show quantity for stackable items
            if (item.MaxStackQuantity > 1)
            {
                itemQuantityText.enabled = true;
                itemQuantityText.text = item.Quantity.ToString();
            }
            else
            {
                itemQuantityText.enabled = false;
            }
        }

        /// <summary>
        /// Called when pointer is pressed on this slot
        /// Initiates drag operation when Alt key is held
        /// </summary>
        /// <param name="eventData">Event data containing pointer information</param>
        public void OnPointerDown(PointerEventData eventData)
        {
            if (item == null || !Input.GetKey(KeyCode.LeftAlt))
            {
                return;
            }

            StartDragging();
        }

        /// <summary>
        /// Sets up visual appearance for dragging an item
        /// </summary>
        private void StartDragging()
        {
            GameManager.Instance.cursor.cursorImage.sprite = item.Icon;
            GameManager.Instance.cursor.cursorImage.enabled = true;
            canvasGroup.alpha = inventory.dragItemAlpha;
        }

        /// <summary>
        /// Called when pointer is released after being pressed on this slot
        /// Handles drag and drop operations including item transfers and dropping
        /// </summary>
        /// <param name="eventData">Event data containing pointer information</param>
        public void OnPointerUp(PointerEventData eventData)
        {
            canvasGroup.alpha = 1;
            if (item == null || !Input.GetKey(KeyCode.LeftAlt))
            {
                return;
            }
        
            EndDragging();
        
            // Get the object under the cursor
            GameObject raycastedObject = eventData.pointerCurrentRaycast.gameObject;
            if (raycastedObject == null)
            {
                // If not over any UI element, drop the item in the world
                inventory.DropItem(item);
                return;
            }
        
            // Check if we're over another inventory slot
            InventorySlot otherSlot = raycastedObject.GetComponent<InventorySlot>();
            if (otherSlot == null)
            {
                return;
            }
        
            // Handle item transfer to the other slot (Empty slot: Move, Same item: Stack, Different item: Swap)
            HandleItemTransfer(otherSlot);
        }

        /// <summary>
        /// Ends the visual dragging state
        /// </summary>
        private void EndDragging()
        {
            if (GameManager.Instance.cursor.cursorImage.sprite == item.Icon)
            {
                GameManager.Instance.cursor.cursorImage.sprite = null;
                GameManager.Instance.cursor.cursorImage.enabled = false;
            }
        }

        /// <summary>
        /// Handles transferring an item to another slot
        /// Implements different behaviors based on destination slot state:
        /// - Empty slot: Move item
        /// - Same item: Stack if possible
        /// - Different item: Swap items
        /// </summary>
        /// <param name="otherSlot">Destination slot for the transfer</param>
        private void HandleItemTransfer(InventorySlot otherSlot)
        {
            if (otherSlot.item == null)
            {
                MoveItemToEmptySlot(otherSlot);
                return;
            }
        
            if (otherSlot.item.ItemName == item.ItemName)
            {
                StackWithSameItem(otherSlot);
                return;
            }
            
            SwapWithDifferentItem(otherSlot);
        }

        /// <summary>
        /// Moves the item to an empty slot
        /// </summary>
        /// <param name="destinationSlot">The empty slot to move to</param>
        private void MoveItemToEmptySlot(InventorySlot destinationSlot)
        {
            if (item.inventorySlot.indexInInventory == inventory.activeItemIndex)
            {
                inventory.SetItemLogicActive(false);
            }
            
            destinationSlot.item = item;
            destinationSlot.item.inventorySlot = destinationSlot;
            item = null;
            UpdateUI();
            destinationSlot.UpdateUI();

            if (destinationSlot.indexInInventory == inventory.activeItemIndex)
            {
                inventory.SetItemLogicActive(true);
            }
        }

        /// <summary>
        /// Attempts to stack this item with the same type of item in another slot
        /// </summary>
        /// <param name="destinationSlot">The slot containing the same item type</param>
        private void StackWithSameItem(InventorySlot destinationSlot)
        {
            int quantityToTransfer = Mathf.Min(item.Quantity,
                destinationSlot.item.MaxStackQuantity - destinationSlot.item.Quantity);
                
            destinationSlot.item.Quantity += quantityToTransfer;
            RemoveFromSlot(quantityToTransfer);
            destinationSlot.UpdateUI();
        }

        /// <summary>
        /// Swaps this item with a different item in another slot
        /// </summary>
        /// <param name="otherSlot">The slot containing a different item</param>
        private void SwapWithDifferentItem(InventorySlot otherSlot)
        {
            InventoryItem tempItem = otherSlot.item;
            otherSlot.item = item;
            otherSlot.item.inventorySlot = otherSlot;
            item = tempItem;
            item.inventorySlot = this;
            
            UpdateUI();
            otherSlot.UpdateUI();

            if (indexInInventory == inventory.activeItemIndex)
            {
                inventory.SetItemLogicActive(otherSlot.indexInInventory, false);
                inventory.SetItemLogicActive(true);
            } else if (otherSlot.indexInInventory == inventory.activeItemIndex)
            {
                inventory.SetItemLogicActive(true);
                inventory.SetItemLogicActive(indexInInventory, false);
            }
        }

        /// <summary>
        /// Removes a specified quantity of items from this slot
        /// Updates the inventory count and destroys the item if quantity reaches zero
        /// </summary>
        /// <param name="quantityToRemove">How many items to remove</param>
        /// <returns>True if the slot is now empty, false otherwise</returns>
        public bool RemoveFromSlot(int quantityToRemove)
        {
            if (item is null)
            {
                return false;
            }
        
            item.Quantity -= quantityToRemove;
            inventory.ChangeItemsCount(item.ItemName, -quantityToRemove);
            
            if (item.Quantity <= 0)
            {
                Destroy(item.gameObject);
                item = null;
                UpdateUI();
                return true;
            }
            
            UpdateUI();
            return false;
        }
    }
}
