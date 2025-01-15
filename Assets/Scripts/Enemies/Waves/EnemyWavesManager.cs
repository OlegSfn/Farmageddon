using System.Collections;
using Data.EnemyWaves;
using Managers;
using UnityEngine;

namespace Enemies.Waves
{
    public class EnemyWavesManager : MonoBehaviour
    {
        [SerializeField] private EnemyWaveData[] wavesData;
        private int _curWaveIndex;
        
        public void SpawnNewWave() => StartCoroutine(SpawnWave());
    
        private IEnumerator SpawnWave()
        {
            EnemyWaveData wave = wavesData[_curWaveIndex];
            int chunkIndex = 0;
            while (chunkIndex < wave.EnemyWaveChunks.Length)
            {
                SpawnChunk(wave.EnemyWaveChunks[chunkIndex]);
                ++chunkIndex;
                yield return new WaitForSeconds(wave.EnemyWaveChunks[chunkIndex].DelayBeforeSpawn);
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
                    Vector3 pos = GameManager.Instance.playerTransform.position + dir * 15;
                    Instantiate(enemyInfo.PrefabToSpawn, pos, Quaternion.identity);
                }
            }
        }
    }
}
