using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace UI.Settings
{
    /// <summary>
    /// Manages the UI for confirming or reverting settings changes
    /// Provides a timed confirmation dialog to apply or cancel changes
    /// </summary>
    public class ApplyChangesMenu : MonoBehaviour
    {
        /// <summary>
        /// Button that applies the pending changes
        /// </summary>
        [SerializeField] private Button applyChangesButton;
        
        /// <summary>
        /// Button that reverts to previous settings
        /// </summary>
        [SerializeField] private Button revertChangesButton;
        
        /// <summary>
        /// Reference to the panels management system
        /// </summary>
        [SerializeField] private PanelsManager panelsManager;
        
        /// <summary>
        /// Time in seconds before changes are automatically reverted
        /// </summary>
        private const int ApplyChangesTimer = 15;

        /// <summary>
        /// Opens the apply changes menu with the specified actions
        /// </summary>
        /// <param name="onApplyChanges">Action to execute when changes are applied</param>
        /// <param name="onRevertChanges">Action to execute when changes are reverted</param>
        public void Open(UnityAction onApplyChanges, UnityAction onRevertChanges)
        {
            applyChangesButton.onClick.AddListener(onApplyChanges);
            revertChangesButton.onClick.AddListener(onRevertChanges);
            
            applyChangesButton.onClick.AddListener(Close);
            revertChangesButton.onClick.AddListener(Close);
            
            panelsManager.OpenPanel(gameObject, Close);
            StartCoroutine(TimerCountdown(onRevertChanges));
        }
        
        /// <summary>
        /// Closes the apply changes menu and cleans up event listeners
        /// </summary>
        public void Close()
        {
            applyChangesButton.onClick.RemoveAllListeners();
            revertChangesButton.onClick.RemoveAllListeners();
            gameObject.SetActive(false);
            StopAllCoroutines();
        }
        
        /// <summary>
        /// Coroutine that counts down before automatically reverting changes
        /// </summary>
        /// <param name="revertAction">Action to execute when the timer expires</param>
        /// <returns>Enumerator for the coroutine system</returns>
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
