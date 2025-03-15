using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace Interactors
{
    public class ColliderInteractor : MonoBehaviour
    {
        [SerializeField] private string[] interactWithTags;
        [SerializeField] private UnityEvent onTriggerEnter;
        [SerializeField] private UnityEvent onTriggerExit;
    
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (interactWithTags.Any(other.CompareTag))
            {
                onTriggerEnter?.Invoke();
            }
        }
    
        private void OnTriggerExit2D(Collider2D other)
        {
            if (interactWithTags.Any(other.CompareTag))
            {
                onTriggerExit?.Invoke();
            }
        }
    }
}
