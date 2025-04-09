using System.Collections.Generic;
using System.Linq;
using ScriptableObjects.Buildings;
using UnityEngine;

namespace Building
{
    /// <summary>
    /// Manages the grid-based placement of buildings in the game world
    /// Tracks building positions and handles spatial queries
    /// </summary>
    public class TilemapManager : MonoBehaviour
    {
        /// <summary>
        /// Dictionary mapping grid positions to lists of buildings at those positions
        /// </summary>
        private readonly Dictionary<Vector2Int, List<Building>> gridBuildings = new();

        /// <summary>
        /// Adds a building to the grid at the specified position
        /// </summary>
        /// <param name="building">The building to add</param>
        /// <param name="position">The grid position where the building should be placed</param>
        /// <returns>True if the building was successfully added, false otherwise</returns>
        public bool AddObject(Building building, Vector2Int position)
        {
            if (!building.buildingData.CanPlace(this, position))
            {
                return false;
            }

            // If the building occupies many cells, we must add it to all of them.
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

        /// <summary>
        /// Removes a building from the grid
        /// </summary>
        /// <param name="building">The building to remove</param>
        /// <param name="position">The grid position of the building</param>
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

        /// <summary>
        /// Gets all buildings at the specified grid position
        /// </summary>
        /// <param name="position">The grid position to check</param>
        /// <returns>A list of buildings at the position (empty if none)</returns>
        public List<Building> GetBuildingsAt(Vector2Int position)
        {
            return gridBuildings.TryGetValue(position, out var objects) ?
                new List<Building>(objects) :
                new List<Building>();
        }

        /// <summary>
        /// Gets the first building at the specified position with the specified BuildingData type
        /// </summary>
        /// <typeparam name="T">The type of BuildingData to look for</typeparam>
        /// <param name="position">The grid position to check</param>
        /// <returns>The matching building, or null if none found</returns>
        public Building GetBuildingAt<T>(Vector2Int position) where T : BuildingData
        {
            return gridBuildings.TryGetValue(position, out var objects) ?
                objects.FirstOrDefault(obj => obj.buildingData is T) :
                null;
        }

        /// <summary>
        /// Checks if a building of a specific type exists at the specified position
        /// </summary>
        /// <typeparam name="T">The type of building to check for</typeparam>
        /// <param name="position">The grid position to check</param>
        /// <returns>True if a matching building exists, false otherwise</returns>
        public bool HasBuildingAt<T>(Vector2Int position)
        {
            return gridBuildings.TryGetValue(position, out var objects) &&
                   objects.Any(obj => obj is T);
        }
    }
}
