using UnityEngine;
using UnityEngine.Events;

public class HealthController : MonoBehaviour
{
    [SerializeField] private int maxHealth;
    [SerializeField] private UnityEvent onValueChange;
    [SerializeField] private UnityEvent onDeath;
    private int _health;
    
    private void Awake()
    {
        _health = maxHealth;
    }
    
    public void TakeDamage(int damage)
    {
        _health -= damage;
        onValueChange?.Invoke();
        if (_health <= 0)
        {
            onDeath?.Invoke();
        }
    }
}
