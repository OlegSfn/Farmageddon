using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace UI.Settings
{
    public class ApplyChangesMenu : MonoBehaviour
    {
        [SerializeField] private Button applyChangesButton;
        [SerializeField] private Button revertChangesButton;
        
        private const int ApplyChangesTimer = 15;

        public void Open(UnityAction onApplyChanges, UnityAction onRevertChanges)
        {
            applyChangesButton.onClick.AddListener(onApplyChanges);
            revertChangesButton.onClick.AddListener(onRevertChanges);
            
            applyChangesButton.onClick.AddListener(Close);
            revertChangesButton.onClick.AddListener(Close);
            
            gameObject.SetActive(true);
            StartCoroutine(TimerCountdown(onRevertChanges));
        }
        
        public void Close()
        {
            applyChangesButton.onClick.RemoveAllListeners();
            revertChangesButton.onClick.RemoveAllListeners();
            StopAllCoroutines();
            gameObject.SetActive(false);
        }
        
        private IEnumerator TimerCountdown(UnityAction revertAction)
        {
            int timeLeft = ApplyChangesTimer;
            while (timeLeft > 0)
            {
                yield return new WaitForSeconds(1f);
                --timeLeft;
            }

            revertAction();
        }
    }
}
