using UnityEngine;

namespace Planting
{
    public class Seedbed : MonoBehaviour
    {
        [SerializeField] private Sprite[] seedBagsSprites;
        [SerializeField] private SpriteRenderer spriteRenderer;

        private void Awake()
        {
            spriteRenderer.sprite = seedBagsSprites[Random.Range(0, seedBagsSprites.Length)];
        }
    
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.TryGetComponent(out Crop crop))
            {
                crop.Humidity = 100;
                crop.Seedbed = this;
            }
        }
    }
}
