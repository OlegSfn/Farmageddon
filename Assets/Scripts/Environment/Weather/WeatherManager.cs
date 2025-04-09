using System.Collections;
using Managers;
using Planting;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Environment.Weather
{
    /// <summary>
    /// Controls the weather system, specifically rain events that affect crop growth
    /// Manages random timing of rain, visual and sound effects and crop humidity
    /// </summary>
    public class WeatherManager : MonoBehaviour
    {
        /// <summary>
        /// Particle system that creates rain visual effect
        /// </summary>
        [SerializeField] private ParticleSystem rain;
        
        /// <summary>
        /// Minimum duration of a rain event in seconds
        /// </summary>
        [SerializeField] private float minRainDuration = 5;
        
        /// <summary>
        /// Maximum duration of a rain event in seconds
        /// </summary>
        [SerializeField] private float maxRainDuration = 30;
        
        /// <summary>
        /// Minimum time between rain events in seconds
        /// </summary>
        [SerializeField] private float minRainInterval = 60;
        
        /// <summary>
        /// Maximum time between rain events in seconds
        /// </summary>
        [SerializeField] private float maxRainInterval = 180;

        /// <summary>
        /// Tracks whether it's currently raining
        /// </summary>
        private bool _isRaining;

        /// <summary>
        /// Start the rain cycle when the game begins
        /// </summary>
        private void Start()
        {
            StartCoroutine(CastNextRain());
        }

        /// <summary>
        /// Updates crop humidity while it's raining
        /// </summary>
        private void Update()
        {
            if (!_isRaining)
            {
                return;
            }
        
            WaterAllCrops();
        }

        /// <summary>
        /// Water all crops while it's raining
        /// </summary>
        private void WaterAllCrops()
        {
            foreach (var crop in FindObjectsByType<Crop>(FindObjectsInactive.Exclude, FindObjectsSortMode.None))
            {
                crop.Humidity += Time.deltaTime * 10;
            }
        }

        /// <summary>
        /// Coroutine that handles a single rain event from start to finish
        /// </summary>
        /// <returns>Coroutine enumerator</returns>
        private IEnumerator PlayRain()
        {
            StartRain();
            yield return new WaitForSeconds(Random.Range(minRainDuration, maxRainDuration));
            StopRain();
        }

        /// <summary>
        /// Stops rain effects and sound
        /// </summary>
        private void StopRain()
        {
            AudioManager.Instance.StopRainSound();
            rain.Stop();
            _isRaining = false;
        }

        /// <summary>
        /// Starts rain effects and sound
        /// </summary>
        private void StartRain()
        {
            rain.Play();
            _isRaining = true;
            AudioManager.Instance.StartRainSound();
        }

        /// <summary>
        /// Continuous coroutine that schedules rain events at random intervals
        /// </summary>
        /// <returns>Coroutine enumerator</returns>
        private IEnumerator CastNextRain()
        {
            while (gameObject.activeInHierarchy)
            {
                yield return new WaitForSeconds(Random.Range(minRainInterval, maxRainInterval));
                StartCoroutine(PlayRain());
            }
        }
    }
}
