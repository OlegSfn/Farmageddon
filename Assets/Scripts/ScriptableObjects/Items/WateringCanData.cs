using UnityEngine;

namespace ScriptableObjects.Items
{
    [CreateAssetMenu(fileName = "WateringCanData", menuName = "ScriptableObjects/WateringCanData")]
    public class WateringCanData : ScriptableObject
    {
        public float distanceMultiplier;
        public int maxWaterAmount;
        public int wateringAmount;
    }
}