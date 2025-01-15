using Planting;
using UnityEngine;

namespace Building
{
    public class CropsObjectPlacer : ObjectPlacer
    {
        private Seedbed _seedbed;

        protected override void UseItem(Vector3Int cursorPosition)
        {
            base.UseItem(cursorPosition);
            Crop crop = PlacedBuilding.GetComponent<Crop>();
            crop.Seedbed = _seedbed;
        }

        protected override bool CheckIfCanUseItem()
        {
            _seedbed = null;
            return base.CheckIfCanUseItem();
        }

        private void GetSeedbed(Collider2D col)
        {
            _seedbed = col.GetComponent<Seedbed>();
        }
        
        private void OnEnable()
        {
            OnIncludeTagFound += GetSeedbed;
        }
        
        private void OnDisable()
        {
            OnIncludeTagFound -= GetSeedbed;
        }
    }
}
