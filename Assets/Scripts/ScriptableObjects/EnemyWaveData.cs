using UnityEngine;
using UnityEngine.Serialization;

namespace ScriptableObjects
{
    [CreateAssetMenu(fileName = "EnemyWaveData", menuName = "ScriptableObjects/Enemies/Wave", order = 1)]
    public class EnemyWaveData : ScriptableObject
    {
        public string waveName;
        public EnemyWaveChunk[] enemyWaveChunks;
    
        [System.Serializable]
        public struct EnemyWaveChunk {
            public EnemiesToSpawn[] enemiesToSpawn;
            public float delayBeforeSpawn;
        }

        [System.Serializable]
        public struct EnemiesToSpawn
        {
            public GameObject prefabToSpawn;
            public int count;
        }
    }
}

