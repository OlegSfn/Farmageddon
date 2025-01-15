using System.Collections.Generic;
using Managers;
using Planting;
using UnityEngine;

namespace Items
{
    public class WateringCan : WorldCursor
    {
        public bool isAboveWaterSource;
    
        [SerializeField] private int waterAmount = 100;
    
        private Crop _selectedCrop;
        private const string CropTag = "Crop";
        private const string WaterSourceTag = "WaterSource";

        protected override void UseItem(Vector3Int cursorPosition)
        {
            WaterCrop();
            RefillWater();
        }

        protected override bool CheckIfCanUseItem()
        {
            _selectedCrop = null;
            isAboveWaterSource = false;
            bool isCloseToPlayer = (CursorGameObject.transform.position - GameManager.Instance.playerTransform.position).sqrMagnitude < GameManager.Instance.sqrDistanceToUseItems;
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
        
            return isCloseToPlayer && (_selectedCrop is not null || isAboveWaterSource);
        }

        private void RefillWater()
        {
            if (!isAboveWaterSource || _selectedCrop is not null) return;
        
            GameManager.Instance.playerContoller.IsWatering = true;
            waterAmount = 100;
        }

        private void WaterCrop()
        {
            if (waterAmount <= 0 || _selectedCrop is null) return;
        
            GameManager.Instance.playerContoller.IsWatering = true;
            waterAmount -= 20;
            _selectedCrop.Humidity += 20;
        }
    }
}
