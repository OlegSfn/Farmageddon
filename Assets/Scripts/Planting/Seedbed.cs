using UnityEngine;

namespace Planting
{
    /// <summary>
    /// Represents a seedbed for planting crops
    /// </summary>
    public class Seedbed : MonoBehaviour
    {
        /// <summary>
        /// Array of sprite variations for visual randomization
        /// </summary>
        [SerializeField] private Sprite[] seedBagsSprites;
        
        /// <summary>
        /// Reference to the seedbed's sprite renderer
        /// </summary>
        [SerializeField] private SpriteRenderer spriteRenderer;

        /// <summary>
        /// Initialize the seedbed with a random visual appearance
        /// </summary>
        private void Awake()
        {
            spriteRenderer.sprite = seedBagsSprites[Random.Range(0, seedBagsSprites.Length)];
        }
    
        /// <summary>
        /// Handles interaction with crops that are planted on this seedbed
        /// Sets initial humidity and establishes the relationship between crop and seedbed
        /// </summary>
        /// <param name="other">The collider that entered this seedbed's trigger area</param>
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