using System;
using Data.Weapons.Swords;
using Managers;
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
                GameManager.Instance.playerContoller.IsAttacking = true;
                GameManager.Instance.playerContoller.ToolAnimator.runtimeAnimatorController = animatorOverrideController;
            }
        }

        public void SetActive(bool active)
        {
            enabled = active;
        }
    }
}
