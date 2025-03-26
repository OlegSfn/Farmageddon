using Interactors;
using Managers;
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

        public void UpdateUI()
        {
            foreach (var shopItem in shopItems)
            {
                shopItem.UpdateUI();
            }
        }
        
        private void PopOutItems()
        {
            float buyDelay = 0;
            float sellDelay = 0;
            foreach (var shopItem in shopItems)
            {
                if (shopItem.isSelling)
                {
                    LeanTween.scale(shopItem.gameObject, Vector3.one, 0.1f).setDelay(sellDelay);
                    sellDelay += 0.05f;
                }
                else
                {
                    LeanTween.scale(shopItem.gameObject, Vector3.one, 0.1f).setDelay(buyDelay);
                    buyDelay += 0.05f;
                }
            }
        }

        private void PopInItems()
        {
            foreach (var shopItem in shopItems)
            {
                shopItem.transform.localScale = Vector3.zero;
            }
        }
    
        public void CloseShop()
        {
            onShopMenuClosed?.Invoke();
            LeanTween.scale(gameObject, Vector3.zero, 0.5f)
                .setEase(LeanTweenType.easeOutExpo)
                .setOnComplete(
                    () =>
                        {
                            gameObject.SetActive(false);
                            PopInItems();
                            
                            if (GameManager.Instance.dayNightManager.IsDay)
                            {
                                shopInteractor.enabled = true;
                                shopInteractorHelper.SetActive(true);
                            }
                        }
                );

        }
    
        public void OpenShop()
        {
            GameManager.Instance.panelsManager.OpenPanel(gameObject, CloseShop);
            UpdateUI();
            onShopMenuOpened?.Invoke();
            LeanTween.scale(gameObject, Vector3.one, 0.5f)
                .setEase(LeanTweenType.easeOutExpo)
                .setOnComplete(
                    () =>
                        {
                            shopInteractor.enabled = false;
                            shopInteractorHelper.SetActive(false);
                        }
                );
            
            LeanTween.delayedCall(gameObject, 0.25f, PopOutItems);
        }
    }
}
