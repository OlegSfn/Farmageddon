using UnityEngine;

namespace ScriptableObjects.Buildings.Concrete
{
    [CreateAssetMenu(fileName = "FenceBuilding", menuName = "ScriptableObjects/Buildings/FenceBuilding", order = 1)]
    public class FenceBuildingData : BuildingData
    {
        public Sprite[] sprites;
    }
}