using Planting;
using ScriptableObjects.Buildings.Concrete;
using UnityEngine;

namespace Building
{
    public class SeedsPlacer : BuildingPlacer
    {
        protected override void UseItem(Vector3Int cursorPosition)
        {
            base.UseItem(cursorPosition);
            Crop crop = PlacedBuilding.GetComponent<Crop>();
            crop.Seedbed = TilemapManager.GetBuildingAt<SeedbedBuildingData>((Vector2Int)GetObjectPosition()).GetComponent<Seedbed>();
        }
    }
}