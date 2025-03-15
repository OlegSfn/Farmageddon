using Building;
using UnityEngine;

namespace ScriptableObjects.Buildings.Concrete
{
    [CreateAssetMenu(fileName = "CropBuilding", menuName = "ScriptableObjects/Buildings/CropBuilding", order = 1)]
    public class CropBuildingData : BuildingData
    {
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