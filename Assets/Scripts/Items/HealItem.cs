using Inventory;
using Managers;
using UnityEngine;

namespace Items
{
    public class HealItem : MonoBehaviour, ILogic
    {
        [SerializeField] private int healAmount;
        [SerializeField] protected InventoryItem item;
        
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                GameManager.Instance.playerHealthController.Heal(healAmount);
                item.RemoveItems(1);
            }
        }
        
        public void SetActive(bool active)
        {
            enabled = active;
        }
    }
}
