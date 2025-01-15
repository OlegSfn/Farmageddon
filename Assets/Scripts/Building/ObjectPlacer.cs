using System.Collections.Generic;
using Inventory;
using Managers;
using UnityEngine;

namespace Building
{
    public class ObjectPlacer : WorldCursor
    {
        [SerializeField] protected GameObject builtPrefab;
        [SerializeField] protected InventoryItem item;
    
        protected GameObject PlacedBuilding;
    
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
            return CursorCollider.Overlap(ContactFilter, colliders) == 0 && isCloseToPlayer;
        }
    }
}
