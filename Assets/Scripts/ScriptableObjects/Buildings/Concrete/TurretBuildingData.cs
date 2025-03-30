using UnityEngine;

namespace ScriptableObjects.Buildings.Concrete
{
    /// <summary>
    /// Defines data for a turret building that automatically targets and shoots at enemies
    /// </summary>
    [CreateAssetMenu(fileName = "Turret", menuName = "ScriptableObjects/Buildings/TurretBuilding", order = 1)]
    public class TurretBuildingData : BuildingData
    {
        public GameObject bulletPrefab;
        public Color bulletColor;
        public float fireCooldown = 1f;
        public int bulletDamage;
        public float bulletSpeed;
    }
}