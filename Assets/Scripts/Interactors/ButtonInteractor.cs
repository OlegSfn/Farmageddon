using Managers;
using UnityEngine;
using UnityEngine.Events;

namespace Interactors
{
    /// <summary>
    /// Simple button interaction component that triggers an event when a specified key is pressed
    /// Used for basic keyboard-based interactions in the game
    /// </summary>
    public class ButtonInteractor : MonoBehaviour
    {
        /// <summary>
        /// Key that triggers the interaction, defaults to E
        /// </summary>
        [SerializeField] private KeyCode interactKey = KeyCode.E;
        
        /// <summary>
        /// Event that gets triggered when the interaction key is pressed
        /// </summary>
        [SerializeField] private UnityEvent onInteract;
    
        /// <summary>
        /// Checks for key press each frame and triggers the interaction event if detected
        /// </summary>
        private void Update()
        {
            if (GameManager.Instance.IsPaused)
            {
                return;
            }
            
            if (Input.GetKeyDown(interactKey))
            {
                onInteract?.Invoke();
            }
        }
    }
}