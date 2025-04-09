using System.Collections;
using System.Linq;
using ScriptableObjects.Crops;
using UnityEngine;

namespace Planting
{
    /// <summary>
    /// Manages a plant crop from planting through growth stages to harvest
    /// Handles growth progression, humidity requirements, and visual representation
    /// </summary>
    public class Crop : MonoBehaviour
    {
        /// <summary>
        /// Reference to the crop's sprite renderer for visual updates
        /// </summary>
        [SerializeField] private SpriteRenderer spriteRenderer;
        
        /// <summary>
        /// Data configuration for this crop type
        /// </summary>
        [SerializeField] private CropData cropData;
        
        /// <summary>
        /// Total time required for the crop to grow through all stages
        /// </summary>
        [SerializeField] private float growthTime;
        
        /// <summary>
        /// Current growth stage index of the crop
        /// </summary>
        private int _currentStage;
    
        /// <summary>
        /// Reference to the seedbed this crop was planted in
        /// </summary>
        public Seedbed Seedbed { get; set; }
        
        /// <summary>
        /// Current humidity level of the crop, affecting growth and appearance
        /// </summary>
        [field: SerializeField] public float Humidity { get; set; }
        
        /// <summary>
        /// Initialize the crop with starting values and begin growth process
        /// </summary>
        private void Start()
        {
            Humidity = cropData.maxHumidity;
            
            growthTime = cropData.growthStagesTimes.Sum();
            
            StartCoroutine(Grow());
        }

        /// <summary>
        /// Updates crop humidity
        /// </summary>
        private void Update()
        {
            HandleHumidity();
        }

        /// <summary>
        /// Manages crop humidity, updates visual appearance, and checks for drying out
        /// </summary>
        private void HandleHumidity()
        {
            Humidity -= Time.deltaTime;
            
            if (Humidity > cropData.maxHumidity)
            {
                Humidity = cropData.maxHumidity;
            }
        
            UpdateCropSprite();
        
            if (Humidity <= 0)
            {
                Die();
            }
        }

        /// <summary>
        /// Updates the crop's visual appearance based on current humidity and growth stage
        /// </summary>
        private void UpdateCropSprite()
        {
            if (2 * Humidity >= cropData.maxHumidity)
            {
                spriteRenderer.sprite = cropData.growthStagesWet[_currentStage];
            }
            else
            {
                spriteRenderer.sprite = cropData.growthStages[_currentStage];
            }
        }

        /// <summary>
        /// Coroutine that progresses the crop through growth stages over time
        /// </summary>
        /// <returns>IEnumerator for coroutine functionality</returns>
        private IEnumerator Grow()
        {
            float startTime = Time.time;
        
            while (Time.time - startTime < growthTime)
            {
                yield return new WaitForSeconds(cropData.growthStagesTimes[_currentStage]);
                
                ++_currentStage;
                // Remove the seedbed once the crop has grown past the first stage (seeds)
                if (_currentStage == 1)
                {
                    Destroy(Seedbed.gameObject);
                }
                
                if (_currentStage >= cropData.growthStages.Length)
                {
                    break;
                }
            }
        
            // Dies from age
            Die();
        }

        /// <summary>
        /// Destroys the crop, called when it dries out or completes its lifecycle
        /// </summary>
        public void Die()
        {
            Destroy(gameObject);
        }

        /// <summary>
        /// Harvests the crop, spawning produce if fully grown and destroying the plant
        /// </summary>
        public void Harvest()
        {
            if (_currentStage == cropData.growthStages.Length - 1)
            {
                Instantiate(cropData.harvestPrefab, transform.position, Quaternion.identity);
            }
            
            Die();
        }
    }
}
