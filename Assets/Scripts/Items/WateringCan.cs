using System.Collections.Generic;
using Managers;
using Planting;
using ScriptableObjects;
using ScriptableObjects.Items;
using UnityEngine;

namespace Items
{
    public class WateringCan : WorldCursor
    {
        public bool isAboveWaterSource;
    
        [SerializeField] protected WateringCanData data;
        
        [SerializeField] protected AnimatorOverrideController animatorOverrideController;
    
        private Crop _selectedCrop;
        private int _waterAmount;

        private const string CropTag = "Crop";
        private const string WaterSourceTag = "WaterSource";

        protected override void Start()
        {
            base.Start();
            ContactFilter.useTriggers = true;
            _waterAmount = data.maxWaterAmount;
        }
        
        protected override void UseItem(Vector3Int cursorPosition)
        {
            WaterCrop();
            RefillWater();
            
            GameManager.Instance.playerController.ToolAnimator.runtimeAnimatorController = animatorOverrideController;
        }

        protected override bool CheckIfCanUseItem()
        {
            _selectedCrop = null;
            isAboveWaterSource = false;
            
            float sqrDistanceToCursor = (GameManager.Instance.playerTransform.position - CursorGameObject.transform.position).sqrMagnitude;
            float maxUseDistance = GameManager.Instance.sqrDistanceToUseItems * data.distanceMultiplier;
            bool isCloseToPlayer = sqrDistanceToCursor <= maxUseDistance;
            
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
        
            return isCloseToPlayer && (_selectedCrop is not null && _waterAmount > 0 || isAboveWaterSource);
        }

        private void RefillWater()
        {
            if (!isAboveWaterSource || _selectedCrop is not null) return;
        
            GameManager.Instance.playerController.IsWatering = true;
            _waterAmount = data.maxWaterAmount;
        }

        private void WaterCrop()
        {
            if (_waterAmount <= 0 || _selectedCrop is null) return;
        
            GameManager.Instance.playerController.IsWatering = true;
            _waterAmount -= data.wateringAmount;
            _selectedCrop.Humidity += data.wateringAmount;
        }
    }
}
