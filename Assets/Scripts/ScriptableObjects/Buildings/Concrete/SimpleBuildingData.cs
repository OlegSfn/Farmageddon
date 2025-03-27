using UnityEngine;

namespace ScriptableObjects.Buildings.Concrete
{
    /// <summary>
    /// Configuration data for basic building types without special placement requirements
    /// Inherits core building functionality from BuildingData
    /// </summary>
    [CreateAssetMenu(fileName = "SimpleBuilding", menuName = "ScriptableObjects/Buildings/SimpleBuilding", order = 1)]
    public class SimpleBuildingData : BuildingData
    {
        // Inherits all basic building functionality from BuildingData
        // No additional parameters required for basic buildings
    }
}