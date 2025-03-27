using System.Collections.Generic;
using Inventory;
using Managers;
using ScriptableObjects.Buildings;
using UnityEngine;

namespace Building
{
    /// <summary>
    /// Handles the placement of buildings in the game world
    /// Shows a preview and validates placement before constructing the actual building
    /// </summary>
    public class BuildingPlacer : WorldCursor
    {
        /// <summary>
        /// The prefab to instantiate when the building is placed
        /// </summary>
        [SerializeField] protected GameObject builtPrefab;
        
        /// <summary>
        /// The inventory item representing this building
        /// </summary>
        [SerializeField] protected InventoryItem item;
        
        /// <summary>
        /// Configuration data for this building type
        /// </summary>
        [SerializeField] protected BuildingData buildingData;

        /// <summary>
        /// Reference to the building that was placed
        /// </summary>
        protected GameObject PlacedBuilding;
        
        /// <summary>
        /// Reference to the tilemap manager for placement validation
        /// </summary>
        protected TilemapManager TilemapManager;
        
        /// <summary>
        /// Initializes the building placer
        /// </summary>
        protected override void Start()
        {
            base.Start();
            TilemapManager = GameManager.Instance.tilemapManager;
        }

        /// <summary>
        /// Places the building at the cursor position and consumes the inventory item
        /// </summary>
        /// <param name="cursorPosition">Position to place the building</param>
        protected override void UseItem(Vector3Int cursorPosition)
        {
            if (item.RemoveItems(1))
            {
                Destroy(CursorGameObject);
            }
            
            PlacedBuilding = Instantiate(builtPrefab, cursorPosition, Quaternion.identity);
        }

        /// <summary>
        /// Validates if a building can be placed at the current cursor position
        /// Checks for player distance, collisions, and building-specific placement rules
        /// </summary>
        /// <returns>True if the building can be placed, false otherwise</returns>
        protected override bool CheckIfCanUseItem()
        {
            bool isCloseToPlayer = (CursorGameObject.transform.position - GameManager.Instance.playerTransform.position).sqrMagnitude < GameManager.Instance.sqrDistanceToUseItems;
            
            List<Collider2D> colliders = new List<Collider2D>();
            CursorCollider.Overlap(ContactFilter, colliders);
            foreach (var col in colliders)
            {
                if (!col.isTrigger)
                {
                    return false;
                }
            }
            
            Vector2Int cursorPosition = (Vector2Int)GetObjectPosition();
            return isCloseToPlayer && buildingData.CanPlace(TilemapManager, cursorPosition);
        }
    }
}