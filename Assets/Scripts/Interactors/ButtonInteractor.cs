using UnityEngine;
using UnityEngine.Events;

namespace Interactors
{
    public class ButtonInteractor : MonoBehaviour
    {
        [SerializeField] private KeyCode interactKey = KeyCode.E;
        [SerializeField] private UnityEvent onInteract;
    
        private void Update()
        {
            if (Input.GetKeyDown(interactKey))
            {
                onInteract?.Invoke();
            }
        }
    }
}
