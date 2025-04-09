using System.Collections.Generic;
using Enemies;
using UnityEngine;

namespace Building.Concrete.Scarecrow
{
    /// <summary>
    /// A building that scares enemies within its trigger area
    /// </summary>
    public class ScarecrowBuilding : Building, IScary
    {
        /// <summary>
        /// Collection of scarable entities currently in range
        /// </summary>
        private readonly HashSet<IScarable> _scarables = new();
        
        /// <summary>
        /// Value that that shows how many enemies can be scared by this scarecrow
        /// Higher values cause more enemies to be scared
        /// </summary>
        [SerializeField] private int scareness = 3;

        /// <summary>
        /// Updates the scare effect for all affected entities
        /// Scare value decreases as more entities are affected
        /// </summary>
        private void UpdateScareForAll()
        {
            int currentScareValue = Mathf.Max(0, scareness - _scarables.Count + 1);
            
            foreach (var scarable in _scarables)
            {
                scarable.UpdateScareFromSource(this, currentScareValue);
            }
        }

        /// <summary>
        /// Adds the entity to the affected list if it can be scared
        /// And if recalculates the scare value for all affected entities
        /// </summary>
        /// <param name="other">The collider of the entity entering the trigger</param>
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag("Enemy") || !other.TryGetComponent(out IScarable scarable))
            {
                return;
            }
            
            _scarables.Add(scarable);
            UpdateScareForAll();
        }

        /// <summary>
        /// Removes the entity from the affected list if it was there and recalculates the
        /// scare value for all affected entities
        /// </summary>
        /// <param name="other">The collider of the entity leaving the trigger</param>
        private void OnTriggerExit2D(Collider2D other)
        {
            if (!other.CompareTag("Enemy") 
                || !other.TryGetComponent(out IScarable scarable)
                || !_scarables.Remove(scarable))
            {
                return;
            }
            
            scarable.RemoveScareFromSource(this);
            UpdateScareForAll();
        }

        /// <summary>
        /// Implements IScary interface to provide the transform of this scare source
        /// </summary>
        /// <returns>The transform of this scarecrow</returns>
        public Transform GetTransform()
        {
            return transform;
        }

        /// <summary>
        /// Destroys this scarecrow, including its parent object
        /// </summary>
        public override void Die()
        {
            Destroy(gameObject.transform.parent.gameObject);
        }
    }
}