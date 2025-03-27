using Managers;
using ScriptableObjects.Items;
using UnityEngine;

namespace Items
{
    /// <summary>
    /// Sword weapon that the player can use to attack enemies
    /// </summary>
    public class Sword : MonoBehaviour, ILogic
    {
        /// <summary>
        /// Sword configuration data
        /// </summary>
        [field: SerializeField] public SwordData swordData { get; set; }
        
        /// <summary>
        /// Animation override controller for the player's sword swing animation
        /// </summary>
        [SerializeField] protected AnimatorOverrideController animatorOverrideController;

        /// <summary>
        /// Checks for left mouse click to initiate a sword attack
        /// Sets the player state to attacking and applies the sword animation
        /// </summary>
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Mouse0) && !Input.GetKeyDown(KeyCode.LeftAlt))
            {
                GameManager.Instance.playerController.IsAttacking = true;
                GameManager.Instance.playerController.ToolAnimator.runtimeAnimatorController = animatorOverrideController;
            }
        }

        /// <summary>
        /// Enables or disables this component when the sword is selected/deselected
        /// Implemented from ILogic interface
        /// </summary>
        /// <param name="active">Whether the sword should be active</param>
        public void SetActive(bool active)
        {
            enabled = active;
        }
    }
}