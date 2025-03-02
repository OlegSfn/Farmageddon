using System.Collections.Generic;
using System.Linq;
using ScriptableObjects.Buildings;
using UnityEngine;

namespace Building
{
    public class TilemapManager : MonoBehaviour
    {
        private readonly Dictionary<Vector2Int, List<Building>> gridBuildings = new();

        public bool AddObject(Building building, Vector2Int position)
        {
            if (!building.buildingData.CanPlace(this, position))
            {
                return false;
            }

            foreach (var tile in building.buildingData.GetOccupiedCells(position))
            {
                if (!gridBuildings.ContainsKey(tile))
                {
                    gridBuildings[tile] = new List<Building>();
                }
            
                gridBuildings[tile].Add(building);
            }

            return true;
        }

        public void RemoveObject(Building building, Vector2Int position)
        {
            foreach (var tile in building.buildingData.GetOccupiedCells(position))
            {
                if (gridBuildings.TryGetValue(tile, out var buildings))
                {
                    buildings.Remove(building);
                }
            }
        }

        public List<Building> GetBuildingsAt(Vector2Int position)
        {
            return gridBuildings.TryGetValue(position, out var objects) ? 
                new List<Building>(objects) : 
                new List<Building>();
        }

        public Building GetBuildingAt<T>(Vector2Int position) where T : BuildingData
        {
            return gridBuildings.TryGetValue(position, out var objects) ? 
                objects.FirstOrDefault(obj => obj.buildingData is T) : 
                null;
        }

        public bool HasBuildingAt<T>(Vector2Int position)
        {
            return gridBuildings.TryGetValue(position, out var objects) && 
                   objects.Any(obj => obj is T);
        }
    }
}
