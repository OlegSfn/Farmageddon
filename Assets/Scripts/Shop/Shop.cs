using Interactors;
using UnityEngine;
using UnityEngine.Events;

namespace Shop
{
    public class Shop : MonoBehaviour
    {
        [SerializeField] private ShopItem[] shopItems;
        [SerializeField] private UnityEvent onShopMenuClosed;
        [SerializeField] private ColliderInteractor shopInteractor;
        [SerializeField] private GameObject shopInteractorHelper;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                onShopMenuClosed?.Invoke();
            }
        }

        public void UpdateUI()
        {
            foreach (var shopItem in shopItems)
            {
                shopItem.UpdateUI();
            }
        }

        private void OnEnable()
        {
            UpdateUI();
        }
    
        public void CloseShop()
        {
            onShopMenuClosed?.Invoke();
            shopInteractor.enabled = false;
            shopInteractorHelper.SetActive(false);
        }
    
        public void OpenShop()
        {
            shopInteractor.enabled = true;
            shopInteractorHelper.SetActive(true);
        }
    }
}
