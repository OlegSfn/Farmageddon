using Inventory;
using Managers;
using UnityEngine;

namespace Items
{
    /// <summary>
    /// Item that heals the player when used
    /// </summary>
    public class HealItem : MonoBehaviour, ILogic
    {
        /// <summary>
        /// Amount of health to restore when used
        /// </summary>
        [SerializeField] private int healAmount;
        
        /// <summary>
        /// Reference to the inventory item component
        /// </summary>
        [SerializeField] protected InventoryItem item;
        
        /// <summary>
        /// Checks for left mouse click and not holding left alt to use the healing item
        /// Heals the player and consumes one item
        /// </summary>
        private void Update()
        {
            if (GameManager.Instance.IsPaused)
            {
                return;
            }

            if (!Input.GetKeyDown(KeyCode.Mouse0) || Input.GetKeyDown(KeyCode.LeftAlt))
            {
                return;
            }
            
            GameManager.Instance.playerHealthController.Heal(healAmount);
            item.RemoveItems(1);
        }
        
        /// <summary>
        /// Enables or disables this component when the item is selected/deselected
        /// Implemented from ILogic interface
        /// </summary>
        /// <param name="active">Whether the item should be active</param>
        public void SetActive(bool active)
        {
            enabled = active;
        }
    }
}