using System.Collections;
using System.Linq;
using Data.Crops;
using UnityEngine;

namespace Planting
{
    public class Crop : MonoBehaviour
    {
        
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private CropData cropData;
        [SerializeField] private float growthTime;
        
        private int _currentStage;
    
        public Seedbed Seedbed { get; set; }
        public float Humidity { get; set; }
        
        private void Start()
        {
            growthTime = cropData.growthStagesTimes.Sum();
            StartCoroutine(Grow());
        }

        private void Update()
        {
            HandleHumidity();
        }

        private void HandleHumidity()
        {
            Humidity -= Time.deltaTime;
            if (Humidity > cropData.maxHumidity)
            {
                Humidity = cropData.maxHumidity;
            }
        
            if (2 * Humidity >= cropData.maxHumidity)
            {
                spriteRenderer.sprite = cropData.growthStagesWet[_currentStage];
            }
            else
            {
                spriteRenderer.sprite = cropData.growthStages[_currentStage];
            }
        
            if (Humidity <= 0)
            {
                Die();
            }
        }

        private IEnumerator Grow()
        {
            float startTime = Time.time;
        
            while (Time.time - startTime < growthTime)
            {
                yield return new WaitForSeconds(cropData.growthStagesTimes[_currentStage]);
                ++_currentStage;
                if (_currentStage == 1)
                {
                    Destroy(Seedbed.gameObject);
                }
                if (_currentStage >= cropData.growthStages.Length)
                {
                    break;
                }
            }
        
            Die();
        }

        public void Die()
        {
            Destroy(gameObject);
        }
    }
}
