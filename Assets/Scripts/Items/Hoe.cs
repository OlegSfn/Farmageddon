using System.Collections.Generic;
using Building;
using Managers;
using Planting;
using ScriptableObjects.Buildings.Concrete;
using ScriptableObjects.Items;
using UnityEngine;

namespace Items
{
    public class Hoe : WorldCursor
    {
        private Crop _selectedCrop;
        private Seedbed _selectedSeedbed;
        [SerializeField] protected GameObject seedbedPrefab;
        
        [SerializeField] protected HoeData data;
        
        [SerializeField] protected AnimatorOverrideController animatorOverrideController;

        private TilemapManager _tilemapManager;
        
        protected override void Start()
        {
            base.Start();
            _tilemapManager = GameManager.Instance.tilemapManager;
        }

        protected override void UseItem(Vector3Int cursorPosition)
        {
            GameManager.Instance.playerController.ToolAnimator.runtimeAnimatorController = animatorOverrideController;
            
            GameManager.Instance.playerController.IsWeeding = true;
            if (_selectedCrop is not null)
            {
                _selectedCrop.Harvest();
            }
            else if (_selectedSeedbed is null)
            {
                Instantiate(seedbedPrefab, cursorPosition, Quaternion.identity);
            }
        }

        protected override bool CheckIfCanUseItem()
        {
            Vector2Int cursorPosition = (Vector2Int)GetObjectPosition();
            _selectedCrop = null;
            _selectedSeedbed = null;
            _tilemapManager.GetBuildingAt<CropBuildingData>(cursorPosition)?.TryGetComponent(out _selectedCrop);
            _tilemapManager.GetBuildingAt<SeedbedBuildingData>(cursorPosition)?.TryGetComponent(out _selectedSeedbed);
            
            float sqrDistanceToCursor = (GameManager.Instance.playerTransform.position - CursorGameObject.transform.position).sqrMagnitude;
            float maxUseDistance = GameManager.Instance.sqrDistanceToUseItems * data.distanceMultiplier;
            bool isCloseToPlayer = sqrDistanceToCursor <= maxUseDistance;
            
            List<Collider2D> colliders = new List<Collider2D>();
            CursorCollider.Overlap(ContactFilter, colliders);
            foreach (var col in colliders)
            {
                if (!col.isTrigger && !col.CompareTag("Player"))
                {
                    return false;
                }
            }
            
            bool isAbleToPlough = isCloseToPlayer && !GameManager.Instance.playerController.IsWeeding;
            return isAbleToPlough && !(_selectedSeedbed is not null && _selectedCrop is null);
        }
    }
}
