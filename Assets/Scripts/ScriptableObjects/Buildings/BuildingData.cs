using System.Collections.Generic;
using System.Linq;
using Building;
using UnityEngine;

namespace ScriptableObjects.Buildings
{
    /// <summary>
    /// Base abstract class for building data configuration
    /// Provides common functionality for all building types
    /// </summary>
    public abstract class BuildingData : ScriptableObject
    {
        /// <summary>
        /// Size of the building in grid cells (width and height)
        /// </summary>
        [field: SerializeField] public Vector2Int Size { get; protected set; } = Vector2Int.one;
        
        /// <summary>
        /// Checks if a building can be placed at the specified position
        /// </summary>
        /// <param name="manager">The tilemap manager to check placement against</param>
        /// <param name="position">The grid position to check</param>
        /// <returns>True if the building can be placed, false otherwise</returns>
        public virtual bool CanPlace(TilemapManager manager, Vector2Int position)
        {
            return GetOccupiedCells(position).All(cell =>
                !manager.GetBuildingsAt(cell).Any());
        }

        /// <summary>
        /// Gets all grid cells that would be occupied by this building if placed at the given position
        /// </summary>
        /// <param name="position">The down-left position of the building</param>
        /// <returns>Collection of all grid cells the building would occupy</returns>
        public virtual IEnumerable<Vector2Int> GetOccupiedCells(Vector2Int position)
        {
            for (int x = 0; x < Size.x; x++)
            {
                for (int y = 0; y < Size.y; y++)
                {
                    yield return position + new Vector2Int(x, y);
                }
            }
        }
    }
}