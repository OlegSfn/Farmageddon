using System.Collections;
using ScriptableObjects;
using UnityEngine;

namespace Enemies.Waves
{
    public class EnemyWavesManager : MonoBehaviour
    {
        [SerializeField] private EnemyWaveData[] wavesData;
        private int _curWaveIndex;
        private float _radius = 35;

        private float _delayMultiplier;
        private float _enemiesCountMultiplier;
        
        public void SpawnNewWave() => StartCoroutine(SpawnWave());

        private EnemyWaveData GetWaveToSpawn()
        {
            if (_curWaveIndex < wavesData.Length)
            {
                return wavesData[_curWaveIndex];
            }

            return wavesData[wavesData.Length - 1];
        }

        private void CalculateMultipliers()
        {
            float extraDayCount = (_curWaveIndex + 1) - wavesData.Length;

            if (extraDayCount <= 0)
            {
                _delayMultiplier = 1;
                _enemiesCountMultiplier = 1;
                return;
            }
            
            _delayMultiplier = Mathf.Max(0, 1 - extraDayCount/30);
            _enemiesCountMultiplier = 1 + extraDayCount/5;
        }
        
        private IEnumerator SpawnWave()
        {
            EnemyWaveData wave = GetWaveToSpawn();
            CalculateMultipliers();

            int chunkIndex = 0;
            while (chunkIndex < wave.EnemyWaveChunks.Length)
            {
                yield return new WaitForSeconds(wave.EnemyWaveChunks[chunkIndex].DelayBeforeSpawn * _delayMultiplier);
                SpawnChunk(wave.EnemyWaveChunks[chunkIndex]);
                ++chunkIndex;
            }

            ++_curWaveIndex;
        }

        private void SpawnChunk(EnemyWaveData.EnemyWaveChunk chunkInfo)
        {
            foreach (var enemyInfo in chunkInfo.EnemiesToSpawn)
            {
                for (int _ = 0; _ < Mathf.Round(enemyInfo.Count*_enemiesCountMultiplier); _++)
                {
                    float angle = Random.Range(0f, 360f);
                    Vector3 dir = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0);
                    Vector3 pos = dir * _radius;
                    Instantiate(enemyInfo.PrefabToSpawn, pos, Quaternion.identity);
                }
            }
        }
    }
}
