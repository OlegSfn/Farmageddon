using System.Collections.Generic;
using Managers;
using UnityEngine;

namespace Inventory
{
    public class Inventory : MonoBehaviour
    {
        [SerializeField] private List<InventorySlot> inventorySlots;
    
        public int activeItemIndex;
    
        public float dragItemAlpha = 0.6f;
    
        public readonly Dictionary<string, int> ItemQuantities = new();

        private void Update()
        {
            if (Input.mouseScrollDelta != Vector2.zero)
            {
                int index = activeItemIndex + (int)Input.mouseScrollDelta.y;
                index = Mathf.Clamp(index, 0, inventorySlots.Count - 1);
                ChangeActiveItem(index);
            }
        
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

        public void AddItems(InventoryItem item, int quantity)
        {
            int quantityToAdd = quantity;
            foreach (var inventorySlot in inventorySlots)
            {
                if (quantityToAdd <= 0)
                {
                    Destroy(item.gameObject);
                    return;
                }
                if (inventorySlot.item != null && inventorySlot.item.ItemName == item.ItemName)
                {
                    int quantityToTransfer = Mathf.Min(quantityToAdd, item.MaxStackQuantity - inventorySlot.item.Quantity);
                    inventorySlot.item.Quantity += quantityToTransfer;
                    ChangeItemsCount(item.ItemName, quantityToTransfer);
                    inventorySlot.UpdateUI();
                    quantityToAdd -= quantityToTransfer;
                }
            }
        
            foreach (var inventorySlot in inventorySlots)
            {
                if (quantityToAdd <= 0)
                {
                    Destroy(item.gameObject);
                    if (inventorySlots[activeItemIndex].item != null)
                    {
                        inventorySlots[activeItemIndex].item.gameObject.GetComponent<ILogic>().SetActive(true);
                    }
                    return;
                }
                if (inventorySlot.item == null)
                {
                    inventorySlot.item = Instantiate(item.gameObject, GameManager.Instance.objectsPool.position, Quaternion.identity).GetComponent<InventoryItem>();
                    inventorySlot.item.Quantity = Mathf.Min(quantityToAdd, item.MaxStackQuantity);
                    ChangeItemsCount(item.ItemName, inventorySlot.item.Quantity);
                    inventorySlot.item.inventorySlot = inventorySlot;
                    inventorySlot.UpdateUI();
                    quantityToAdd -= inventorySlot.item.Quantity;
                }
            }
        
            while(quantityToAdd > 0)
            {
                Vector2 randomPosition = (Vector2)GameManager.Instance.playerTransform.position + Random.insideUnitCircle * 0.5f;
                InventoryItem newItem = Instantiate(item.gameObject, randomPosition, Quaternion.identity).GetComponent<InventoryItem>();
                newItem.Quantity = Mathf.Min(quantityToAdd, item.MaxStackQuantity);
                quantityToAdd -= newItem.Quantity;
            }
        
            Destroy(item.gameObject);
        }
    
        public void DropItem(InventoryItem item)
        {
            if (item.inventorySlot == null)
            {
                return;
            }
        
            ChangeItemsCount(item.ItemName, -item.Quantity);
            item.inventorySlot.item = null;
            item.inventorySlot.UpdateUI();
            item.inventorySlot = null;
            Vector2 randomPosition = (Vector2)GameManager.Instance.playerTransform.position + Random.insideUnitCircle * 0.5f;
            item.transform.position = randomPosition;
            item.StartCoroutine(item.HandleDroppingItem());
        }


        private void ChangeActiveItem(int index)
        {
            if (activeItemIndex == index)
            {
                return;
            }
        
            inventorySlots[activeItemIndex].slotBorder.enabled = false;
            inventorySlots[index].slotBorder.enabled = true;
        
            if (inventorySlots[activeItemIndex].item is not null)
            {
                inventorySlots[activeItemIndex].item.gameObject.GetComponent<ILogic>().SetActive(false);
            }
            activeItemIndex = index;
            if (inventorySlots[activeItemIndex].item is not null)
            {
                inventorySlots[activeItemIndex].item.gameObject.GetComponent<ILogic>().SetActive(true);
            }
        }
    
    
        public void RemoveItems(string itemName, int quantity)
        {
            int quantityToRemove = quantity;
            for (int i = inventorySlots.Count-1; i >= 0; --i)
            {
                if (inventorySlots[i].item is not null && itemName == inventorySlots[i].item.ItemName)
                {
                    int itemQuantity = inventorySlots[i].item.Quantity;
                    inventorySlots[i].RemoveFromSlot(Mathf.Min(quantityToRemove, itemQuantity));
                    quantityToRemove -= itemQuantity;
                }
            }
        }
    
        public void ChangeItemsCount(string itemName, int count)
        {
            if (count > 0)
            {
                if (!ItemQuantities.TryAdd(itemName, count))
                {
                    ItemQuantities[itemName] += count;
                }
            }
            else
            {
                ItemQuantities[itemName] += count;
                if (ItemQuantities[itemName] <= 0)
                {
                    ItemQuantities.Remove(itemName);
                }
            }
        }
    }
}