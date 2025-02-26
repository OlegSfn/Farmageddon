using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(fileName = "EnemyWaveData", menuName = "ScriptableObjects/Enemies/Wave", order = 1)]
    public class EnemyWaveData : ScriptableObject
    {
        public string WaveName;
        public EnemyWaveChunk[] EnemyWaveChunks;
    
        [System.Serializable]
        public struct EnemyWaveChunk {
            public EnemiesToSpawn[] EnemiesToSpawn;
            public float DelayBeforeSpawn;
        }

        [System.Serializable]
        public struct EnemiesToSpawn
        {
            public GameObject PrefabToSpawn;
            public int Count;
        }
    }
}

