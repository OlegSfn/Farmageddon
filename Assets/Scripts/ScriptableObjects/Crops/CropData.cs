using UnityEngine;

namespace ScriptableObjects.Crops
{
    /// <summary>
    /// Defines configuration data for crop types
    /// Contains visual assets and growth parameters for plant lifecycle
    /// </summary>
    [CreateAssetMenu(fileName = "CropData", menuName = "ScriptableObjects/Crops/Crop", order = 1)]
    public class CropData : ScriptableObject
    {
        /// <summary>
        /// Visual sprites for each growth stage of the plant (dry state)
        /// </summary>
        public Sprite[] growthStages;
        
        /// <summary>
        /// Visual sprites for each growth stage when the plant is watered
        /// </summary>
        public Sprite[] growthStagesWet;
        
        /// <summary>
        /// Time duration (in days or game time) for each growth stage
        /// </summary>
        public float[] growthStagesTimes;
        
        /// <summary>
        /// Maximum water content the plant can hold
        /// </summary>
        public int maxHumidity;

        /// <summary>
        /// Prefab to spawn when the crop is harvested
        /// </summary>
        public GameObject harvestPrefab;
    }
}