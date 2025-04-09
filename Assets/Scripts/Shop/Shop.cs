using Interactors;
using Managers;
using UnityEngine;
using UnityEngine.Events;

namespace Shop
{
    /// <summary>
    /// Manages the shop interface and interactions
    /// Handles opening/closing animations, item display, and interaction state
    /// </summary>
    public class Shop : MonoBehaviour
    {
        /// <summary>
        /// Array of items available in this shop
        /// </summary>
        [SerializeField] private ShopItem[] shopItems;
        
        /// <summary>
        /// Event triggered when the shop menu is opened
        /// </summary>
        [SerializeField] private UnityEvent onShopMenuOpened;
        
        /// <summary>
        /// Event triggered when the shop menu is closed
        /// </summary>
        [SerializeField] private UnityEvent onShopMenuClosed;
        
        /// <summary>
        /// Interactor component that allows player to trigger the shop
        /// </summary>
        [SerializeField] private ColliderInteractor shopInteractor;
        
        /// <summary>
        /// Visual indicator showing the shop can be interacted with
        /// </summary>
        [SerializeField] private GameObject shopInteractorHelper;

        /// <summary>
        /// Updates the display of all items in the shop
        /// </summary>
        public void UpdateUI()
        {
            foreach (var shopItem in shopItems)
            {
                shopItem.UpdateUI();
            }
        }
        
        /// <summary>
        /// Animates shop items appearing with a staggered delay
        /// Buy and sell items are animated in separate groups
        /// </summary>
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

        /// <summary>
        /// Resets all shop items to zero scale to prepare for animations
        /// </summary>
        private void PopInItems()
        {
            foreach (var shopItem in shopItems)
            {
                shopItem.transform.localScale = Vector3.zero;
            }
        }
    
        /// <summary>
        /// Closes the shop with animation and restores interaction ability
        /// </summary>
        private void CloseShop()
        {
            onShopMenuClosed?.Invoke();
            
            LeanTween.scale(gameObject, Vector3.zero, 0.5f)
                .setEase(LeanTweenType.easeOutExpo)
                .setOnComplete(
                    () =>
                        {
                            gameObject.SetActive(false);
                            PopInItems();

                            if (!GameManager.Instance.dayNightManager.IsDay)
                            {
                                return;
                            }
                            
                            shopInteractor.enabled = true;
                            shopInteractorHelper.SetActive(true);
                        }
                );

        }
    
        /// <summary>
        /// Opens the shop with animation and disables the shop interactor
        /// </summary>
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
            
            // Delay item appearance for a smoother opening sequence
            LeanTween.delayedCall(gameObject, 0.25f, PopOutItems);
        }
    }
}
