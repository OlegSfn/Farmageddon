using System.Collections;
using ScriptableObjects.Enemies;
using UnityEngine;

namespace Enemies.Waves
{
    /// <summary>
    /// Manages the spawning of enemy waves with increasing difficulty over time
    /// Handles wave progression, timing, and enemy placement
    /// </summary>
    public class EnemyWavesManager : MonoBehaviour
    {
        /// <summary>
        /// Array of wave configurations in order of appearance
        /// </summary>
        [SerializeField] private EnemyWaveData[] wavesData;
        
        /// <summary>
        /// Index of the current wave
        /// </summary>
        private int _curWaveIndex;
        
        /// <summary>
        /// Distance from center at which enemies spawn
        /// </summary>
        private float _radius = 35;

        /// <summary>
        /// Reduces spawn delay between chunks for later waves
        /// </summary>
        private float _delayMultiplier;
        
        /// <summary>
        /// Increases enemy count for later waves
        /// </summary>
        private float _enemiesCountMultiplier;
        
        /// <summary>
        /// Initiates the spawning of a new wave of enemies
        /// </summary>
        public void SpawnNewWave() => StartCoroutine(SpawnWave());

        /// <summary>
        /// Determines which wave data to use for the current wave
        /// After all defined waves, reuses the final wave with increasing difficulty
        /// </summary>
        /// <returns>The wave data to spawn</returns>
        private EnemyWaveData GetWaveToSpawn()
        {
            if (_curWaveIndex < wavesData.Length)
            {
                return wavesData[_curWaveIndex];
            }

            return wavesData[wavesData.Length - 1];
        }

        /// <summary>
        /// Calculates difficulty multipliers based on current wave index
        /// Makes waves progressively harder after all defined waves have been used
        /// </summary>
        private void CalculateMultipliers()
        {
            float extraDayCount = (_curWaveIndex + 1) - wavesData.Length;

            // Use normal values for predefined waves
            if (extraDayCount <= 0)
            {
                _delayMultiplier = 1;
                _enemiesCountMultiplier = 1;
                return;
            }
            
            // For waves beyond predefined data:
            // - Reduce delay between spawns (up to 100% reduction after 30 extra days)
            // - Increase enemy count (20% more per 5 extra days)
            _delayMultiplier = Mathf.Max(0, 1 - extraDayCount/30);
            _enemiesCountMultiplier = 1 + extraDayCount/5;
        }
        
        /// <summary>
        /// Coroutine that handles the spawning sequence of a wave
        /// Spawns enemy chunks with appropriate delays between them
        /// </summary>
        /// <returns>Coroutine enumerator</returns>
        private IEnumerator SpawnWave()
        {
            EnemyWaveData wave = GetWaveToSpawn();
            CalculateMultipliers();

            int chunkIndex = 0;
            while (chunkIndex < wave.enemyWaveChunks.Length)
            {
                yield return new WaitForSeconds(wave.enemyWaveChunks[chunkIndex].delayBeforeSpawn * _delayMultiplier);
                SpawnChunk(wave.enemyWaveChunks[chunkIndex]);
                ++chunkIndex;
            }

            ++_curWaveIndex;
        }

        /// <summary>
        /// Spawns a group of enemies defined in a chunk
        /// Places enemies in a circle around the center point
        /// </summary>
        /// <param name="chunkInfo">Data defining which enemies to spawn and how many</param>
        private void SpawnChunk(EnemyWaveData.EnemyWaveChunk chunkInfo)
        {
            foreach (var enemyInfo in chunkInfo.enemiesToSpawn)
            {
                int enemiesToSpawn = Mathf.RoundToInt(enemyInfo.count * _enemiesCountMultiplier);
                
                for (int _ = 0; _ < enemiesToSpawn; _++)
                {
                    SpawnEnemyOnCircle(enemyInfo.prefabToSpawn);
                }
            }
        }

        /// <summary>
        /// Spawns an enemy at a random point on a circle around the center
        /// </summary>
        /// <param name="enemyToSpawn">Enemy prefab to spawn on a circle</param>
        private void SpawnEnemyOnCircle(GameObject enemyToSpawn)
        {
            float angle = Random.Range(0f, 360f);
            Vector3 dir = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0);
            Vector3 pos = dir * _radius;
            Instantiate(enemyToSpawn, pos, Quaternion.identity);
        }
    }
}
