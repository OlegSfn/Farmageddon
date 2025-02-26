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
        
        public void SpawnNewWave() => StartCoroutine(SpawnWave());
    
        private IEnumerator SpawnWave()
        {
            EnemyWaveData wave = wavesData[_curWaveIndex];
            int chunkIndex = 0;
            while (chunkIndex < wave.EnemyWaveChunks.Length)
            {
                yield return new WaitForSeconds(wave.EnemyWaveChunks[chunkIndex].DelayBeforeSpawn);
                SpawnChunk(wave.EnemyWaveChunks[chunkIndex]);
                ++chunkIndex;
            }

            ++_curWaveIndex;
        }

        private void SpawnChunk(EnemyWaveData.EnemyWaveChunk chunkInfo)
        {
            foreach (var enemyInfo in chunkInfo.EnemiesToSpawn)
            {
                for (int _ = 0; _ < enemyInfo.Count; _++)
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
