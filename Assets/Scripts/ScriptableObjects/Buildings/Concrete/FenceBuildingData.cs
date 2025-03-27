using UnityEngine;

namespace ScriptableObjects.Buildings.Concrete
{
    /// <summary>
    /// Configuration data for fence building structures
    /// Contains visual appearance options for different fence states
    /// </summary>
    [CreateAssetMenu(fileName = "FenceBuilding", menuName = "ScriptableObjects/Buildings/FenceBuilding", order = 1)]
    public class FenceBuildingData : BuildingData
    {
        /// <summary>
        /// Array of fence sprites used for different connection states
        /// Used to show appropriate fence visuals based on adjacent fences
        /// </summary>
        public Sprite[] sprites;
    }
}