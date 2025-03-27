using Building;
using UnityEngine;

namespace ScriptableObjects.Buildings.Concrete
{
    /// <summary>
    /// Defines data for a crop building that can only be placed on seedbeds
    /// Handles validation of placement requirements on the farm
    /// </summary>
    [CreateAssetMenu(fileName = "CropBuilding", menuName = "ScriptableObjects/Buildings/CropBuilding", order = 1)]
    public class CropBuildingData : BuildingData
    {
        /// <summary>
        /// Checks if a crop can be placed at the specified position
        /// Crops can only be placed on seedbeds and cannot overlap other crops
        /// </summary>
        /// <param name="manager">The tilemap manager to check placement against</param>
        /// <param name="position">The grid position to check</param>
        /// <returns>True if the crop can be placed, false otherwise</returns>
        public override bool CanPlace(TilemapManager manager, Vector2Int position)
        {
            foreach (var cell in GetOccupiedCells(position))
            {
                var objects = manager.GetBuildingsAt(cell);
                bool hasGardenBed = false;
            
                foreach (var obj in objects)
                {
                    if (obj.buildingData is SeedbedBuildingData) hasGardenBed = true;
                    if (obj.buildingData is CropBuildingData) return false;
                }
            
                if (!hasGardenBed) return false;
            }
            return true;
        }
    }
}