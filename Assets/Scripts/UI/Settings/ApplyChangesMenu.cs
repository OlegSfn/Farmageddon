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
        [SerializeField] private PanelsManager panelsManager;
        
        private const int ApplyChangesTimer = 15;

        public void Open(UnityAction onApplyChanges, UnityAction onRevertChanges)
        {
            applyChangesButton.onClick.AddListener(onApplyChanges);
            revertChangesButton.onClick.AddListener(onRevertChanges);
            
            applyChangesButton.onClick.AddListener(Close);
            revertChangesButton.onClick.AddListener(Close);
            
            panelsManager.OpenPanel(gameObject, Close);
            StartCoroutine(TimerCountdown(onRevertChanges));
        }
        
        public void Close()
        {
            applyChangesButton.onClick.RemoveAllListeners();
            revertChangesButton.onClick.RemoveAllListeners();
            gameObject.SetActive(false);
            StopAllCoroutines();
        }
        
        private IEnumerator TimerCountdown(UnityAction revertAction)
        {
            int timeLeft = ApplyChangesTimer;
            while (timeLeft > 0)
            {
                yield return new WaitForSecondsRealtime(1f);
                --timeLeft;
            }

            revertAction();
        }
    }
}
