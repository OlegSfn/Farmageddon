using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    [SerializeField] private int maxHealth;
    [SerializeField] private UnityEvent onDeath;
    private int _health;
    
    private void Awake()
    {
        _health = maxHealth;
    }
    
    public void TakeDamage(int damage)
    {
        _health -= damage;
        if (_health <= 0)
        {
            onDeath?.Invoke();
        }
    }
}
