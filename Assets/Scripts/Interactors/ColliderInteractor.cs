using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace Interactors
{
    /// <summary>
    /// Handles physics-based interactions through 2D collider triggers
    /// Triggers events when objects with specific tags enter or exit the collider
    /// </summary>
    public class ColliderInteractor : MonoBehaviour
    {
        /// <summary>
        /// Array of tags that this interactor will respond to
        /// </summary>
        [SerializeField] private string[] interactWithTags;
        
        /// <summary>
        /// Event triggered when an object with a matching tag enters the collider
        /// </summary>
        [SerializeField] private UnityEvent onTriggerEnter;
        
        /// <summary>
        /// Event triggered when an object with a matching tag exits the collider
        /// </summary>
        [SerializeField] private UnityEvent onTriggerExit;
    
        /// <summary>
        /// Checks if the entering object has a relevant tag and invokes the enter event if so
        /// </summary>
        /// <param name="other">The collider that entered this trigger</param>
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (interactWithTags.Any(other.CompareTag))
            {
                onTriggerEnter?.Invoke();
            }
        }
    
        /// <summary>
        /// Checks if the exiting object has a relevant tag and invokes the exit event if so
        /// </summary>
        /// <param name="other">The collider that exited this trigger</param>
        private void OnTriggerExit2D(Collider2D other)
        {
            if (interactWithTags.Any(other.CompareTag))
            {
                onTriggerExit?.Invoke();
            }
        }
    }
}