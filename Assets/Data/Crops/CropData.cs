using UnityEngine;

namespace Data.Crops
{
    [CreateAssetMenu(fileName = "CropData", menuName = "ScriptableObjects/Crops/Crop", order = 1)]
    public class CropData : ScriptableObject
    {
        public Sprite[] growthStages;
        public Sprite[] growthStagesWet;
        public float[] growthStagesTimes;
        public int maxHumidity;
    }
}