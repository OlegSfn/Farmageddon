using System.Collections.Generic;
using System.Linq;
using Building;
using UnityEngine;

namespace ScriptableObjects.Buildings
{
    public abstract class BuildingData : ScriptableObject
    {
        public Vector2Int Size { get; protected set; } = Vector2Int.one;
        
        public virtual bool CanPlace(TilemapManager manager, Vector2Int position)
        {
            return GetOccupiedCells(position).All(cell => 
                !manager.GetBuildingsAt(cell).Any());
        }

        public virtual IEnumerable<Vector2Int> GetOccupiedCells(Vector2Int position)
        {
            for (int x = 0; x < Size.x; x++)
            {
                for (int y = 0; y < Size.y; y++)
                {
                    yield return position + new Vector2Int(x, y);
                }
            }
        }
    }
}