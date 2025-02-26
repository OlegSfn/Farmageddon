using Interactors;
using UnityEngine;
using UnityEngine.Events;

namespace Shop
{
    public class Shop : MonoBehaviour
    {
        [SerializeField] private ShopItem[] shopItems;
        [SerializeField] private UnityEvent onShopMenuOpened;
        [SerializeField] private UnityEvent onShopMenuClosed;
        [SerializeField] private ColliderInteractor shopInteractor;
        [SerializeField] private GameObject shopInteractorHelper;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                CloseShop();
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
            gameObject.SetActive(false);
            onShopMenuClosed?.Invoke();
            shopInteractor.enabled = true;
            shopInteractorHelper.SetActive(true);
        }
    
        public void OpenShop()
        {
            gameObject.SetActive(true);
            onShopMenuOpened?.Invoke();
            shopInteractor.enabled = false;
            shopInteractorHelper.SetActive(false);
        }
    }
}
