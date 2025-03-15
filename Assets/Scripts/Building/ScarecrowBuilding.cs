using System.Collections.Generic;
using Enemies;
using UnityEngine;

namespace Building
{
    public class ScarecrowBuilding : Building, IScary
    {
        private HashSet<IScarable> _scarables = new();
        [SerializeField] private int scareness = 3;

        private void UpdateScareForAll()
        {
            int currentScareValue = Mathf.Max(0, scareness - _scarables.Count + 1);
            foreach (var scarable in _scarables)
            {
                scarable.UpdateScareFromSource(this, currentScareValue);
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Enemy") && other.TryGetComponent(out IScarable scarable))
            {
                _scarables.Add(scarable);
                UpdateScareForAll();
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Enemy") && other.TryGetComponent(out IScarable scarable))
            {
                if (_scarables.Remove(scarable))
                {
                    scarable.RemoveScareFromSource(this);
                    UpdateScareForAll();
                }
            }
        }

        public Transform GetTransform()
        {
            return transform;
        }

        public override void Die()
        {
            Destroy(gameObject.transform.parent.gameObject);
        }
    }
}