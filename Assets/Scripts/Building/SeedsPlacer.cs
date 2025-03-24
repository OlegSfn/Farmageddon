using System.Collections.Generic;
using Managers;
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

        protected override bool CheckIfCanUseItem()
        {
            bool isCloseToPlayer = (CursorGameObject.transform.position - GameManager.Instance.playerTransform.position).sqrMagnitude < GameManager.Instance.sqrDistanceToUseItems;
            Vector2Int cursorPosition = (Vector2Int)GetObjectPosition();
            return isCloseToPlayer && buildingData.CanPlace(TilemapManager, cursorPosition);
        }
    }
}