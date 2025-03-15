using UnityEngine;
using UnityEngine.Events;

public class HealthController : MonoBehaviour
{
    [field: SerializeField] public int MaxHealth { get; private set; }
    [SerializeField] private UnityEvent<HitInfo?, int> onValueChange;
    [SerializeField] private UnityEvent onDeath;
    public int _health;
    
    private void Awake()
    {
        _health = MaxHealth;
    }
    
    public void TakeDamage(HitInfo hitInfo)
    {
        _health -= hitInfo.Damage;
        onValueChange?.Invoke(hitInfo, _health);
        if (_health <= 0)
        {
            onDeath?.Invoke();
        }
    }

    public void Heal(int amount)
    {
        _health += amount;
        if (_health > MaxHealth)
        {
            _health = MaxHealth;
        }
        
        onValueChange?.Invoke(null, _health);
    }
}
