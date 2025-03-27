using System.Collections;
using UnityEngine;

namespace Inventory
{
    /// <summary>
    /// Represents an item that can be picked up, stored in inventory, and stacked
    /// Contains core item properties like name, quantity, and icon
    /// </summary>
    public class InventoryItem : MonoBehaviour
    {
        /// <summary>
        /// Time in seconds that an item cannot be picked up after being dropped
        /// Prevents accidental immediate pickup after dropping
        /// </summary>
        [SerializeField] private float unpickableTime = 1f;
    
        /// <summary>
        /// Reference to the inventory slot containing this item, null if not in inventory
        /// </summary>
        [HideInInspector] public InventorySlot inventorySlot;

        /// <summary>
        /// Whether the item can currently be picked up
        /// </summary>
        public bool IsPickable { get; private set; } = true;
        
        /// <summary>
        /// Display name of the item
        /// </summary>
        [field: SerializeField] public string ItemName { get; private set; }
        
        /// <summary>
        /// Maximum number of this item that can be stacked in one inventory slot
        /// </summary>
        [field: SerializeField] public int MaxStackQuantity { get; private set; } = 1;
        
        /// <summary>
        /// Current quantity of this item instance
        /// </summary>
        [field: SerializeField] public int Quantity { get; set; } = 1;
        
        /// <summary>
        /// Icon sprite used to represent this item in the UI
        /// </summary>
        [field: SerializeField] public Sprite Icon { get; private set; }

        /// <summary>
        /// Removes a specified quantity of this item
        /// If the item is in an inventory slot, delegates to the slot's removal method
        /// Otherwise reduces the quantity directly and destroys if quantity reaches zero
        /// </summary>
        /// <param name="quantityToRemove">How many items to remove</param>
        /// <returns>True if the item was completely removed, false if some quantity remains</returns>
        public bool RemoveItems(int quantityToRemove)
        {
            // If the item is in inventory, let the inventory slot handle removal
            if (inventorySlot is not null)
            {
                return inventorySlot.RemoveFromSlot(quantityToRemove);
            }
            
            // Otherwise handle direct removal (for items in the world)
            Quantity -= quantityToRemove;
            if (Quantity <= 0)
            {
                Destroy(gameObject);
                return true;
            }

            return false;
        }
    
        /// <summary>
        /// Coroutine that handles the temporary unpickable state after an item is dropped
        /// Makes the item unpickable for a short duration, then pickable again
        /// </summary>
        /// <returns>Coroutine enumerator</returns>
        public IEnumerator HandleDroppingItem()
        {
            IsPickable = false;
            float startTime = Time.time;

            while (Time.time - startTime < unpickableTime)
            {
                yield return new WaitForSeconds(unpickableTime/10);
            }

            IsPickable = true;
        }

        /// <summary>
        /// Destroys this item
        /// Called when the item is consumed or otherwise removed from the game
        /// </summary>
        public void Die()
        {
            Destroy(gameObject);
        }
    }
}
