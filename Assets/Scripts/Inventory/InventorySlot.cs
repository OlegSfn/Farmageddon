using Managers;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Inventory
{
    public class InventorySlot : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField] private Inventory inventory;
        public InventoryItem item;
        public Image slotBorder;
        [SerializeField] private Image itemIcon;
        [SerializeField] private TextMeshProUGUI itemQuantityText;
        [SerializeField] private CanvasGroup canvasGroup;

        private void Awake()
        {
            UpdateUI();
        }

        public void UpdateUI()
        {
            if (item == null)
            {
                itemIcon.enabled = false;
                itemQuantityText.enabled = false;
                return;
            }
        
            itemIcon.enabled = true;
            itemIcon.sprite = item.Icon;
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

        public void OnPointerDown(PointerEventData eventData)
        {
            if (item == null || !Input.GetKey(KeyCode.LeftAlt))
            {
                return;
            }

            GameManager.Instance.cursor.cursorImage.sprite = item.Icon;
            GameManager.Instance.cursor.cursorImage.enabled = true;
            canvasGroup.alpha = inventory.dragItemAlpha;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            canvasGroup.alpha = 1;
            if (item == null || !Input.GetKey(KeyCode.LeftAlt))
            {
                return;
            }
        
            if (GameManager.Instance.cursor.cursorImage.sprite == item.Icon)
            {
                GameManager.Instance.cursor.cursorImage.sprite = null;
                GameManager.Instance.cursor.cursorImage.enabled = false;
            }
        
            GameObject raycastedObject = eventData.pointerCurrentRaycast.gameObject;
            if (raycastedObject == null)
            {
                inventory.DropItem(item);
                return;
            }
        
            InventorySlot otherSlot = raycastedObject.GetComponent<InventorySlot>(); 
            if (otherSlot == null)
            {
                return;
            }
        
            if (otherSlot.item == null)
            {
                otherSlot.item = item;
                otherSlot.item.inventorySlot = otherSlot;
                item = null;
                UpdateUI();
                otherSlot.UpdateUI();
                return;
            }
        
            if (otherSlot.item.ItemName == item.ItemName)
            {
                int quantityToTransfer = Mathf.Min(item.Quantity, otherSlot.item.MaxStackQuantity - otherSlot.item.Quantity);
                otherSlot.item.Quantity += quantityToTransfer;
                RemoveFromSlot(quantityToTransfer);
                otherSlot.UpdateUI();
                return;
            }
        
            InventoryItem tempItem = otherSlot.item;
            otherSlot.item = item;
            otherSlot.item.inventorySlot = otherSlot;
            item = tempItem;
            item.inventorySlot = this;
            UpdateUI();
            otherSlot.UpdateUI();
        }

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
