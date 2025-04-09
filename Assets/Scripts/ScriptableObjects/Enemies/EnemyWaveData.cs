using UnityEngine;

namespace ScriptableObjects.Enemies
{
    /// <summary>
    /// ScriptableObject that defines an enemy wave configuration
    /// Used to spawn groups of enemies in a timed sequence
    /// </summary>
    [CreateAssetMenu(fileName = "EnemyWaveData", menuName = "ScriptableObjects/Enemies/Wave", order = 1)]
    public class EnemyWaveData : ScriptableObject
    {
        /// <summary>
        /// Display name for this wave
        /// </summary>
        public string waveName;
        
        /// <summary>
        /// Array of enemy chunks that comprise this wave
        /// Each chunk is spawned as a group with its own timing
        /// </summary>
        public EnemyWaveChunk[] enemyWaveChunks;
    
        /// <summary>
        /// Defines a group of enemies to be spawned together at a specific time
        /// </summary>
        [System.Serializable]
        public struct EnemyWaveChunk {
            /// <summary>
            /// Array of enemy types and quantities to spawn in this chunk
            /// </summary>
            public EnemiesToSpawn[] enemiesToSpawn;
            
            /// <summary>
            /// Time in seconds to wait before spawning this chunk
            /// </summary>
            public float delayBeforeSpawn;
        }

        /// <summary>
        /// Defines a specific enemy type and quantity to spawn
        /// </summary>
        [System.Serializable]
        public struct EnemiesToSpawn
        {
            /// <summary>
            /// The enemy prefab to instantiate
            /// </summary>
            public GameObject prefabToSpawn;
            
            /// <summary>
            /// How many of this enemy type to spawn
            /// </summary>
            public int count;
        }
    }
}