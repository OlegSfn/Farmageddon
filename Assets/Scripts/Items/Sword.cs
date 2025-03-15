using System;
using Managers;
using ScriptableObjects;
using ScriptableObjects.Items;
using UnityEngine;

namespace Items
{
    public class Sword : MonoBehaviour, ILogic
    {
        [field: SerializeField] public SwordData swordData { get; set; }
        
        [SerializeField] protected AnimatorOverrideController animatorOverrideController;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                GameManager.Instance.playerController.IsAttacking = true;
                GameManager.Instance.playerController.ToolAnimator.runtimeAnimatorController = animatorOverrideController;
            }
        }

        public void SetActive(bool active)
        {
            enabled = active;
        }
    }
}
