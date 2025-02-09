using System.Collections.Generic;
using Managers;
using Planting;
using UnityEngine;

namespace Items
{
    public class Hoe : WorldCursor
    {
        private Crop _selectedCrop;
        private Seedbed _selectedSeedbag;
        [SerializeField] protected GameObject seedbedPrefab;

        protected override void Start()
        {
            base.Start();
            ContactFilter.useTriggers = true;
        }

        protected override void UseItem(Vector3Int cursorPosition)
        {
            GameManager.Instance.playerContoller.IsWeeding = true;
            if (_selectedCrop is not null)
            {
                _selectedCrop.Harvest();
            }
            else if (_selectedSeedbag is null)
            {
                Instantiate(seedbedPrefab, cursorPosition, Quaternion.identity);
            }
        }

        protected override bool CheckIfCanUseItem()
        {
            _selectedCrop = null;
            _selectedSeedbag = null;
            bool isCloseToPlayer = (CursorGameObject.transform.position - GameManager.Instance.playerTransform.position).sqrMagnitude < GameManager.Instance.sqrDistanceToUseItems;
            List<Collider2D> colliders = new List<Collider2D>();
            CursorCollider.Overlap(ContactFilter, colliders);

            foreach (var col in colliders)
            {
                if (col.gameObject.isStatic && !col.isTrigger)
                {
                    return false;
                }
                
                if (col.CompareTag("Crop"))
                {
                    _selectedCrop = col.GetComponent<Crop>();
                }
                else if (col.CompareTag("Seedbed"))
                {
                    _selectedSeedbag = col.GetComponent<Seedbed>();
                }
            }

            bool isAbleToPlough = isCloseToPlayer && !GameManager.Instance.playerContoller.IsWeeding;
            return isAbleToPlough && !(_selectedSeedbag is not null && _selectedCrop is null);
        }
    }
}
