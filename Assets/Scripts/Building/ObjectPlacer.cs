using System;
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

        [SerializeField] protected string[] includeTagsForBuilding;
        [SerializeField] protected string[] excludeTagsForBuilding;
    
        protected GameObject PlacedBuilding;
        protected Action<Collider2D> OnIncludeTagFound;
        
        private HashSet<string> _includeTags;
        private HashSet<string> _excludeTags;
        
        protected override void Start()
        {
            base.Start();
            _includeTags = new HashSet<string>(includeTagsForBuilding);
            _excludeTags = new HashSet<string>(excludeTagsForBuilding);
            ContactFilter.useTriggers = true;
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
    
            bool hasIncludedTag = _includeTags.Count == 0;
            bool hasExcludedTag = false;
            foreach (var col in colliders)
            {
                if (!col.isTrigger)
                {
                    return false;
                }
                
                if (_includeTags.Contains(col.tag))
                {
                    hasIncludedTag = true;
                    OnIncludeTagFound?.Invoke(col);
                }
                else if (_excludeTags.Contains(col.tag))
                {
                    hasExcludedTag = true;
                    break;
                }
            }
            
            return hasIncludedTag && !hasExcludedTag && isCloseToPlayer;
        }
    }
}
