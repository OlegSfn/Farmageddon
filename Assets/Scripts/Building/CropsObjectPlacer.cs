using System.Collections.Generic;
using Managers;
using Planting;
using UnityEngine;

namespace Building
{
    public class CropsObjectPlacer : ObjectPlacer
    {
        private const string SeedbagTag = "Seedbed";
        private Seedbed _seedbed;

        protected override void Start()
        {
            base.Start();
            ContactFilter.useTriggers = true;
        }
    
        protected override void UseItem(Vector3Int cursorPosition)
        {
            base.UseItem(cursorPosition);
            Crop crop = PlacedBuilding.GetComponent<Crop>();
            crop.Seedbed = _seedbed;
        }

        protected override bool CheckIfCanUseItem()
        {
            _seedbed = null;
            Vector3 playerPosition = GameManager.Instance.playerTransform.position;
            bool isCloseToPlayer = (CursorGameObject.transform.position - playerPosition).sqrMagnitude < GameManager.Instance.sqrDistanceToUseItems;
            List<Collider2D> colliders = new List<Collider2D>();
            CursorCollider.Overlap(ContactFilter, colliders);
        
            bool hasOverlap = false;
            foreach (var col in colliders)
            {
                if (col.CompareTag(SeedbagTag))
                {
                    _seedbed = col.GetComponent<Seedbed>();
                }
                else
                {
                    if (!col.isTrigger)
                    {
                        hasOverlap = true;
                    }
                    break;
                }
            }
        
            return _seedbed is not null && !hasOverlap && isCloseToPlayer;
        }
    }
}
