using System.Collections.Generic;
using Managers;
using Planting;
using ScriptableObjects.Items;
using UnityEngine;

namespace Items
{
    /// <summary>
    /// Watering can tool that can water crops to increase their humidity
    /// and be refilled at water sources
    /// </summary>
    public class WateringCan : WorldCursor
    {
        /// <summary>
        /// Indicates whether the watering can is currently positioned over a water source
        /// </summary>
        public bool isAboveWaterSource;
    
        /// <summary>
        /// Watering can configuration data
        /// </summary>
        [SerializeField] protected WateringCanData data;
        
        /// <summary>
        /// Animation override controller for the player's watering animation
        /// </summary>
        [SerializeField] protected AnimatorOverrideController animatorOverrideController;
    
        /// <summary>
        /// Currently selected crop that can be watered
        /// </summary>
        private Crop _selectedCrop;
        
        /// <summary>
        /// Current water amount in the watering can
        /// </summary>
        private int _waterAmount;

        /// <summary>
        /// Tag used to identify crops in the scene
        /// </summary>
        private const string CropTag = "Crop";
        
        /// <summary>
        /// Tag used to identify water sources in the scene
        /// </summary>
        private const string WaterSourceTag = "WaterSource";

        /// <summary>
        /// Initialize components and set initial water amount
        /// </summary>
        protected override void Start()
        {
            base.Start();
            ContactFilter.useTriggers = true;
            _waterAmount = data.maxWaterAmount;
        }
        
        /// <summary>
        /// Performs the watering can action at the cursor position
        /// Either waters a crop or refills the can based on context
        /// </summary>
        /// <param name="cursorPosition">Position where the watering can is being used</param>
        protected override void UseItem(Vector3Int cursorPosition)
        {
            WaterCrop();
            RefillWater();
            
            GameManager.Instance.playerController.ToolAnimator.runtimeAnimatorController = animatorOverrideController;
        }

        /// <summary>
        /// Checks if the watering can can be used at the current cursor position
        /// </summary>
        /// <returns>True if the watering can can be used, false otherwise</returns>
        protected override bool CheckIfCanUseItem()
        {
            bool isCloseToPlayer = IsPlayerWithinRange();
            
            DetectInteractables();
            
            return isCloseToPlayer && (_selectedCrop is not null && _waterAmount > 0 || isAboveWaterSource);
        }

        /// <summary>
        /// Checks if the player is within range to use the watering can
        /// </summary>
        /// <returns>True if player is close enough, false otherwise</returns>
        private bool IsPlayerWithinRange()
        {
            float sqrDistanceToCursor = (GameManager.Instance.playerTransform.position - CursorGameObject.transform.position).sqrMagnitude;
            float maxUseDistance = GameManager.Instance.sqrDistanceToUseItems * data.distanceMultiplier;
            return sqrDistanceToCursor <= maxUseDistance;
        }

        /// <summary>
        /// Detects crops and water sources at the cursor position
        /// </summary>
        private void DetectInteractables()
        {
            _selectedCrop = null;
            isAboveWaterSource = false;
            List<Collider2D> colliders = new List<Collider2D>();
            CursorCollider.Overlap(ContactFilter, colliders);

            foreach (var col in colliders)
            {
                if (col.CompareTag(CropTag))
                {
                    _selectedCrop = col.GetComponent<Crop>();
                    break;
                }

                if (col.CompareTag(WaterSourceTag))
                {
                    isAboveWaterSource = true;
                    break;
                }
            }
        }

        /// <summary>
        /// Refills the watering can if positioned over a water source
        /// Sets the player's state to watering
        /// </summary>
        private void RefillWater()
        {
            if (!isAboveWaterSource || _selectedCrop is not null)
            {
                return;
            }
        
            GameManager.Instance.playerController.IsWatering = true;
            _waterAmount = data.maxWaterAmount;
        }

        /// <summary>
        /// Waters the selected crop if the watering can has water
        /// Increases crop humidity and reduces water amount
        /// </summary>
        private void WaterCrop()
        {
            if (_waterAmount <= 0 || _selectedCrop is null)
            {
                return;
            }
        
            GameManager.Instance.playerController.IsWatering = true;
            
            int waterAmount = Mathf.Min(_waterAmount, data.wateringAmount);
            _waterAmount -= waterAmount;
            
            _selectedCrop.Humidity += waterAmount;
        }
    }
}
