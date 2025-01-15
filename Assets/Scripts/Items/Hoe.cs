using Building;
using Managers;
using UnityEngine;

namespace Items
{
    public class Hoe : ObjectPlacer
    {
        protected override void UseItem(Vector3Int cursorPosition)
        {
            GameManager.Instance.playerContoller.IsWeeding = true;
            Instantiate(builtPrefab, cursorPosition, Quaternion.identity);
        }

        protected override bool CheckIfCanUseItem()
        {
            return base.CheckIfCanUseItem() && !GameManager.Instance.playerContoller.IsWeeding;
        }
    }
}
