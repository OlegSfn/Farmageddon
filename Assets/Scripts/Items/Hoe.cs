using System.Collections.Generic;
using Building;
using Managers;
using Planting;
using ScriptableObjects.Buildings.Concrete;
using ScriptableObjects.Items;
using UnityEngine;

namespace Items
{
    /// <summary>
    /// Hoe tool that can harvest crops and create new seedbeds for planting
    /// </summary>
    public class Hoe : WorldCursor
    {
        /// <summary>
        /// Currently selected crop that can be harvested
        /// </summary>
        private Crop _selectedCrop;
        
        /// <summary>
        /// Currently selected seedbed
        /// </summary>
        private Seedbed _selectedSeedbed;
        
        /// <summary>
        /// Prefab used to create new seedbeds
        /// </summary>
        [SerializeField] protected GameObject seedbedPrefab;
        
        /// <summary>
        /// Hoe configuration data
        /// </summary>
        [SerializeField] protected HoeData data;
        
        /// <summary>
        /// Animation override controller for the player's weeding animation
        /// </summary>
        [SerializeField] protected AnimatorOverrideController animatorOverrideController;

        /// <summary>
        /// Reference to the tilemap manager for building placement
        /// </summary>
        private TilemapManager _tilemapManager;
        
        /// <summary>
        /// Initialize components on start
        /// </summary>
        protected override void Start()
        {
            base.Start();
            _tilemapManager = GameManager.Instance.tilemapManager;
        }

        /// <summary>
        /// Performs the hoe action at the cursor position
        /// Either harvests a crop, creates a new seedbed, or does nothing based on context
        /// </summary>
        /// <param name="cursorPosition">Position where the hoe is being used</param>
        protected override void UseItem(Vector3Int cursorPosition)
        {
            if (GameManager.Instance.IsPaused)
            {
                return;
            }
            
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

        /// <summary>
        /// Checks if the hoe can be used at the current cursor position
        /// </summary>
        /// <returns>True if the hoe can be used, false otherwise</returns>
        protected override bool CheckIfCanUseItem()
        {
            FindTargetsAtCursor();
            
            bool isCloseToPlayer = IsPlayerWithinRange();
            bool noObstructions = CheckForObstructions();
            bool isAbleToPlough = isCloseToPlayer && !GameManager.Instance.playerController.IsWeeding && noObstructions;
            
            return isAbleToPlough && !(_selectedSeedbed is not null && _selectedCrop is null);
        }
        
        /// <summary>
        /// Finds crop and seedbed targets at the current cursor position
        /// </summary>
        private void FindTargetsAtCursor()
        {
            Vector2Int cursorPosition = (Vector2Int)GetObjectPosition();
            _selectedCrop = null;
            _selectedSeedbed = null;
            
            _tilemapManager.GetBuildingAt<CropBuildingData>(cursorPosition)?.TryGetComponent(out _selectedCrop);
            _tilemapManager.GetBuildingAt<SeedbedBuildingData>(cursorPosition)?.TryGetComponent(out _selectedSeedbed);
        }
        
        /// <summary>
        /// Checks if the player is within range to use the hoe
        /// </summary>
        /// <returns>True if player is close enough, false otherwise</returns>
        private bool IsPlayerWithinRange()
        {
            float sqrDistanceToCursor = (GameManager.Instance.playerTransform.position - CursorGameObject.transform.position).sqrMagnitude;
            float maxUseDistance = GameManager.Instance.sqrDistanceToUseItems * data.distanceMultiplier;
            return sqrDistanceToCursor <= maxUseDistance;
        }
        
        /// <summary>
        /// Checks for physical obstructions that would prevent using the hoe
        /// </summary>
        /// <returns>True if no obstructions, false if something is blocking</returns>
        private bool CheckForObstructions()
        {
            List<Collider2D> colliders = new List<Collider2D>();
            CursorCollider.Overlap(ContactFilter, colliders);
            
            foreach (var col in colliders)
            {
                if (!col.isTrigger && !col.CompareTag("Player"))
                {
                    return false;
                }
            }
            
            return true;
        }
    }
}
