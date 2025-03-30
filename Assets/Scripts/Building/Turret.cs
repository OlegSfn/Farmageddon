using System.Collections;
using System.Collections.Generic;
using ScriptableObjects.Buildings.Concrete;
using UnityEngine;

namespace Building
{
    /// <summary>
    /// Represents a turret building that automatically targets and shoots at enemies
    /// </summary>
    public class Turret : Building
    {
        [SerializeField] private Transform firePoint;
        [SerializeField] private Collider2D attackSightArea;
        
        private ContactFilter2D _contactFilter2D;
        private TurretBuildingData _turretData;

        /// <summary>
        /// Initializes the turret with the turret building data
        /// and starts the target scanning coroutine
        /// </summary>
        private void Start()
        {
            _contactFilter2D.NoFilter();
            _turretData = (TurretBuildingData)buildingData;
            StartCoroutine(FindTarget());
        }
        
        
        /// <summary>
        /// Periodically scans for potential targets within detection range
        /// </summary>
        private IEnumerator FindTarget()
        {
            while (gameObject.activeInHierarchy)
            {
                yield return new WaitForSeconds(_turretData.fireCooldown);
                Shoot();
            }
        }
        
        /// <summary>
        /// Attempts to find a target in attack range based on priority settings
        /// </summary>
        /// <returns>Transform of the highest priority target, or null if none found</returns>
        private Transform GetTarget()
        {
            List<Collider2D> colliders = new List<Collider2D>();
            attackSightArea.Overlap(_contactFilter2D, colliders);
            return GetMaxPriorityTarget(colliders);
        }
        
        /// <summary>
        /// Finds the highest priority enemy target based on distance
        /// </summary>
        /// <param name="colliders">List of colliders in detection range</param>
        /// <returns>Transform of the highest priority target, or null if none found</returns>
        private Transform GetMaxPriorityTarget(List<Collider2D> colliders)
        {
            Transform currentTarget = null;
            float currentDistance = int.MaxValue;
            foreach (var col in colliders)
            {
                if (!col.CompareTag("Enemy"))
                {
                    continue;
                }
                
                float candidateDistance = Vector2.SqrMagnitude(transform.position - col.transform.position);
                if (candidateDistance < currentDistance)
                {
                    currentTarget = col.transform;
                    currentDistance = candidateDistance;
                }
            }

            return currentTarget;
        }
        
        /// <summary>
        /// Shoot with bullet prefab in the direction of the target if one exists
        /// </summary>
        private void Shoot()
        {
            Transform target = GetTarget();
            if (target is null)
            {
                return;
            }
            
            GameObject bulletObject = Instantiate(_turretData.bulletPrefab, firePoint.position, Quaternion.identity);
            Bullet bullet = bulletObject.GetComponent<Bullet>();
            bullet.Initialize(
                (target.position - transform.position).normalized,
                _turretData.bulletDamage,
                _turretData.bulletSpeed,
                _turretData.bulletColor);
        }
    }
}
