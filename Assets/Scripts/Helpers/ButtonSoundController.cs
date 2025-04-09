using Managers;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Helpers
{
    public class ButtonSoundController : MonoBehaviour, IPointerEnterHandler, IPointerDownHandler
    {
        public void OnPointerEnter(PointerEventData eventData)
        {
            AudioManager.Instance.PlayButtonHoverSound();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            AudioManager.Instance.PlayButtonClickSound();
        }
    }
}