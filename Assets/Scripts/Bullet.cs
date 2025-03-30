using UnityEngine;

/// <summary>
/// Represents a bullet projectile that deals damage to enemies and destroys
/// itself on collision or after a set time
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]
public class Bullet : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    private Vector2 moveDirection;
    private int damage;
    private float speed;

    private float liveTime = 5f;
    
    /// <summary>
    /// Handles movement and destruction of the bullet
    /// </summary>
    private void Update()
    {
        transform.Translate(speed * Time.deltaTime * moveDirection);
        liveTime -= Time.deltaTime;
        if (liveTime < 0 || transform.position.sqrMagnitude > 3000)
        {
            Destroy(gameObject);
        }
    }
    
    /// <summary>
    /// Handles collision with enemies and deals damage to them
    /// </summary>
    /// <param name="other">The collider of the entity entering the trigger</param>
    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Collided");
        if (other.CompareTag("Enemy"))
        {
            other.GetComponent<HealthController>().TakeDamage(new HitInfo(damage, transform.position));
            Destroy(gameObject);
        }
    }
    
    /// <summary>
    /// Initializes the bullet with the given parameters
    /// </summary>
    /// <param name="moveDirection">Direction in which the bullet moves</param>
    /// <param name="damage">Damage that bullet will deal to enemies</param>
    /// <param name="speed">Speed at which the bullet moves</param>
    /// <param name="color">Color of the bullet sprite</param>
    public void Initialize(Vector2 moveDirection, int damage, float speed, Color color)
    {
        this.moveDirection = moveDirection;
        this.damage = damage;
        this.speed = speed;
        spriteRenderer.color = color;
    }
}
