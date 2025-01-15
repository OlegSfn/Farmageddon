using System.Collections;
using UnityEngine;

namespace Inventory
{
    public class InventoryItem : MonoBehaviour
    {
        [SerializeField] private float unpickableTime = 1f;
    
        [HideInInspector] public InventorySlot inventorySlot;

        public bool IsPickable { get; private set; } = true;
        
        [field: SerializeField] public string ItemName { get; private set; }
        [field: SerializeField] public int MaxStackQuantity { get; private set; } = 1;
        [field: SerializeField] public int Quantity { get; set; } = 1;
        [field: SerializeField] public Sprite Icon { get; private set; }

        
        public bool RemoveItems(int quantityToRemove)
        {
            if (inventorySlot is not null)
            {
                return inventorySlot.RemoveFromSlot(quantityToRemove);
            }
        
            Quantity -= quantityToRemove;
            if (Quantity <= 0)
            {
                Destroy(gameObject);
                return true;
            }

            return false;
        }
    
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

        public void Die()
        {
            Destroy(gameObject);
        }
    }
}
