using System.Linq;
using ScriptableObjects.Buildings.Concrete;
using UnityEngine;

namespace Building
{
    /// <summary>
    /// Represents a fence building that changes appearance based on adjacent fences
    /// Automatically connects to neighboring fence buildings with appropriate sprites
    /// </summary>
    public class FenceBuilding : Building
    {
        /// <summary>
        /// The sprite renderer component used to display the fence visuals
        /// </summary>
        [SerializeField] private SpriteRenderer spriteRenderer;

        /// <summary>
        /// Specific data for the fence building type
        /// </summary>
        private FenceBuildingData _fenceBuildingData;
        
        /// <summary>
        /// Initializes the fence building and updates its appearance
        /// </summary>
        protected override void Awake()
        {
            base.Awake();
            _fenceBuildingData = (FenceBuildingData)buildingData;
            UpdateAppearance();
            UpdateNeighbors();
        }
        
        /// <summary>
        /// Updates the fence sprite based on adjacent fence buildings
        /// Uses a bitmask to select the appropriate sprite from the fence data
        /// </summary>
        public void UpdateAppearance()
        {
            bool up = TilemapManager.HasBuildingAt<FenceBuilding>(Position + Vector2Int.up);
            bool down = TilemapManager.HasBuildingAt<FenceBuilding>(Position + Vector2Int.down);
            bool left = TilemapManager.HasBuildingAt<FenceBuilding>(Position + Vector2Int.left);
            bool right = TilemapManager.HasBuildingAt<FenceBuilding>(Position + Vector2Int.right);

            // Create a bitmask representing the connection pattern (0-15).
            int spriteIndex = (down ? 1 : 0) | (left ? 2 : 0) | (right ? 4 : 0) | (up ? 8 : 0);
            spriteRenderer.sprite = _fenceBuildingData.sprites[Mathf.Clamp(spriteIndex, 0, _fenceBuildingData.sprites.Length-1)];
        }

        /// <summary>
        /// Updates the appearance of all adjacent fence buildings
        /// Called when this fence is placed or removed to maintain visual consistency
        /// </summary>
        private void UpdateNeighbors()
        {
            Vector2Int[] directions = {
                Vector2Int.up, Vector2Int.down,
                Vector2Int.left, Vector2Int.right
            };

            foreach (var dir in directions)
            {
                foreach (var fence in TilemapManager.GetBuildingsAt(Position + dir).OfType<FenceBuilding>())
                {
                    fence.UpdateAppearance();
                }
            }
        }
    }
}