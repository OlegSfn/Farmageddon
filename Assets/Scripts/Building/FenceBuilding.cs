using System.Linq;
using ScriptableObjects.Buildings.Concrete;
using UnityEngine;

namespace Building
{
    public class FenceBuilding : Building
    {
        [SerializeField] private SpriteRenderer spriteRenderer;

        private FenceBuildingData _fenceBuildingData;
        
        protected override void Awake()
        {
            base.Awake();
            _fenceBuildingData = (FenceBuildingData)buildingData;
            UpdateAppearance();
            UpdateNeighbors();
        }
        
        public void UpdateAppearance()
        {
            bool up = TilemapManager.HasBuildingAt<FenceBuilding>(Position + Vector2Int.up);
            bool down = TilemapManager.HasBuildingAt<FenceBuilding>(Position + Vector2Int.down);
            bool left = TilemapManager.HasBuildingAt<FenceBuilding>(Position + Vector2Int.left);
            bool right = TilemapManager.HasBuildingAt<FenceBuilding>(Position + Vector2Int.right);

            int spriteIndex = (down ? 1 : 0) | (left ? 2 : 0) | (right ? 4 : 0) | (up ? 8 : 0);
            spriteRenderer.sprite = _fenceBuildingData.sprites[Mathf.Clamp(spriteIndex, 0, _fenceBuildingData.sprites.Length-1)];
        }

        private void UpdateNeighbors()
        {
            Vector2Int[] directions = {
                Vector2Int.up, Vector2Int.down, 
                Vector2Int.left, Vector2Int.right
            };

            foreach (var dir in directions)
            {
                foreach (var fence in TilemapManager.GetBuildingsAt(Position + dir).OfType<FenceBuilding>())
                {
                    fence.UpdateAppearance();
                }
            }
        }
    }
}