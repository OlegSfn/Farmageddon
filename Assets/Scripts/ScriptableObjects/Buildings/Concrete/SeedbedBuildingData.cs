using UnityEngine;

namespace ScriptableObjects.Buildings.Concrete
{
    /// <summary>
    /// Configuration data for seedbed building structures
    /// Seedbeds are the foundation for planting crops in the farm
    /// </summary>
    [CreateAssetMenu(fileName = "SeedbedBuilding", menuName = "ScriptableObjects/Buildings/SeedbedBuilding", order = 1)]
    public class SeedbedBuildingData : BuildingData
    {
        // Uses base BuildingData functionality without additions
        // This class exists to create a specific building type that crops can be planted on
    }
}