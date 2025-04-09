using Managers;
using Planting;
using ScriptableObjects.Buildings.Concrete;
using UnityEngine;

namespace Building.Concrete.Seeds
{
    /// <summary>
    /// Specialized building placer for planting seeds in seedbeds
    /// Links the placed crop to its seedbed
    /// </summary>
    public class SeedsPlacer : BuildingPlacer
    {
        /// <summary>
        /// Places the seed at the cursor position and links it to the seedbed below
        /// </summary>
        /// <param name="cursorPosition">Position to place the seed</param>
        protected override void UseItem(Vector3Int cursorPosition)
        {
            base.UseItem(cursorPosition);
            
            Crop crop = PlacedBuilding.GetComponent<Crop>();
            crop.Seedbed = TilemapManager.GetBuildingAt<SeedbedBuildingData>((Vector2Int)GetObjectPosition()).GetComponent<Seedbed>();
        }

        /// <summary>
        /// Validates if a seed can be planted at the current cursor position
        /// Checks for player distance and seeds-specific placement rules
        /// </summary>
        /// <returns>True if the seed can be planted, false otherwise</returns>
        protected override bool CheckIfCanUseItem()
        {
            bool isCloseToPlayer = (CursorGameObject.transform.position - GameManager.Instance.playerTransform.position).sqrMagnitude < GameManager.Instance.sqrDistanceToUseItems;
            
            Vector2Int cursorPosition = (Vector2Int)GetObjectPosition();
            return isCloseToPlayer && buildingData.CanPlace(TilemapManager, cursorPosition);
        }
    }
}