using System.Collections.Generic;
using Inventory;
using Managers;
using ScriptableObjects.Buildings;
using UnityEngine;

namespace Building
{
    public class BuildingPlacer : WorldCursor
    {
        [SerializeField] protected GameObject builtPrefab;
        [SerializeField] protected InventoryItem item;
        [SerializeField] protected BuildingData buildingData;

        protected GameObject PlacedBuilding;
        protected TilemapManager TilemapManager;
        
        protected override void Start()
        {
            base.Start();
            TilemapManager = GameManager.Instance.tilemapManager;
        }

        protected override void UseItem(Vector3Int cursorPosition)
        {
            if (item.RemoveItems(1))
            {
                Destroy(CursorGameObject);
            }
            
            PlacedBuilding = Instantiate(builtPrefab, cursorPosition, Quaternion.identity);
        }

        protected override bool CheckIfCanUseItem()
        {
            bool isCloseToPlayer = (CursorGameObject.transform.position - GameManager.Instance.playerTransform.position).sqrMagnitude < GameManager.Instance.sqrDistanceToUseItems;
            List<Collider2D> colliders = new List<Collider2D>();
            CursorCollider.Overlap(ContactFilter, colliders);
            
            foreach (var col in colliders)
            {
                if (!col.isTrigger && !col.CompareTag("Player"))
                {
                    return false;
                }
            }
            
            Vector2Int cursorPosition = (Vector2Int)GetObjectPosition();
            return isCloseToPlayer && buildingData.CanPlace(TilemapManager, cursorPosition);
        }
    }
}