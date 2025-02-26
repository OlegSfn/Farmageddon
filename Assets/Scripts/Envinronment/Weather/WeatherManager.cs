using System.Collections;
using Planting;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Envinronment
{
    public class WeatherManager : MonoBehaviour
    {
        [SerializeField] private ParticleSystem rain;
        [SerializeField] private float minRainDuration = 5;
        [SerializeField] private float maxRainDuration = 30;
        [SerializeField] private float minRainInterval = 60;
        [SerializeField] private float maxRainInterval = 180;

        private bool _isRaining;

        private void Start()
        {
            StartCoroutine(CastNextRain());
        }

        private void Update()
        {
            if (!_isRaining)
            {
                return;
            }
        
            foreach (var crop in FindObjectsByType<Crop>(FindObjectsInactive.Exclude, FindObjectsSortMode.None))
            {
                crop.Humidity += Time.deltaTime * 10;
            }
        }

        private IEnumerator PlayRain()
        {
            rain.Play();
            _isRaining = true;
            yield return new WaitForSeconds(Random.Range(minRainDuration, maxRainDuration));
            rain.Stop();
            _isRaining = false;
        }

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
