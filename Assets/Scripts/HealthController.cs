using UnityEngine;
using UnityEngine.Events;

public class HealthController : MonoBehaviour
{
    [SerializeField] private int maxHealth;
    [SerializeField] private UnityEvent<HitInfo> onValueChange;
    [SerializeField] private UnityEvent onDeath;
    private int _health;
    
    private void Awake()
    {
        _health = maxHealth;
    }
    
    public void TakeDamage(HitInfo hitInfo)
    {
        _health -= hitInfo.Damage;
        onValueChange?.Invoke(hitInfo);
        if (_health <= 0)
        {
            onDeath?.Invoke();
        }
    }
}
