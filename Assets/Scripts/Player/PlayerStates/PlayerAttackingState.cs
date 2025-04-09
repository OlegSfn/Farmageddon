using System.Collections.Generic;
using Helpers;
using Items;
using Managers;
using UnityEngine;

namespace Player.PlayerStates
{
    /// <summary>
    /// Handles the player's attacking state
    /// Manages the attack animation, collision detection, and damage dealing
    /// </summary>
    public class PlayerAttackingState : PlayerBaseState
    {
        /// <summary>
        /// Name of this state for state machine to check on equality
        /// </summary>
        public override string Name => "Attacking";

        /// <summary>
        /// Collider used to detect hit enemies during attack
        /// </summary>
        private readonly Collider2D toolCollider;
        
        /// <summary>
        /// Filter for collision detection
        /// </summary>
        private readonly ContactFilter2D contactFilter;
        
        /// <summary>
        /// Minimum time between consecutive attacks
        /// </summary>
        private const float AttackCooldown = 0.01f;
        
        /// <summary>
        /// Timestamp of the last attack
        /// </summary>
        private float _lastAttackTime;

        /// <summary>
        /// Flag indicating whether the state can be exited before animation completes
        /// </summary>
        private bool _canExit;
        
        /// <summary>
        /// Constructor initializing the attacking state with necessary components
        /// </summary>
        /// <param name="playerController">Reference to the player controller</param>
        /// <param name="animator">Reference to the player animator</param>
        /// <param name="toolAnimator">Reference to the tool animator</param>
        /// <param name="rigidbody2D">Reference to the player's rigidbody</param>
        /// <param name="toolCollider">Collider used for hit detection</param>
        public PlayerAttackingState(PlayerController playerController, Animator animator, Animator toolAnimator, Rigidbody2D rigidbody2D, Collider2D toolCollider) : base(playerController, animator, toolAnimator, rigidbody2D)
        {
            this.toolCollider = toolCollider;
            contactFilter = new ContactFilter2D();
            contactFilter.NoFilter();
        }
        
        /// <summary>
        /// Called when entering the attacking state
        /// Sets up animation and initializes state variables
        /// </summary>
        public override void OnEnter()
        {
            base.OnEnter();
            _canExit = false;
            Rigidbody2D.linearVelocity = Vector2.zero;
            Animator.CrossFade(AttackingAnimHash, CrossFadeTime);
            ToolAnimator.CrossFade(UseToolAnimHash, CrossFadeTime);
        }

        /// <summary>
        /// Checks for player input to potentially exit the state early
        /// </summary>
        public override void OnUpdate()
        {
            base.OnUpdate();
            // Allow early exit from attack state if player is providing movement input
            // and the animation has reached a point where early exit is allowed
            if (PlayerController.Input != Vector2.zero && _canExit)
            {
                PlayerController.IsAttacking = false;
            }
        }

        /// <summary>
        /// When the animation event is received, the player will try to perform an action
        /// - ProcessAttack: Try to attack
        /// - StopAttacking: End of attack animation
        /// - CanExit: Point in animation where early exit is allowed
        /// </summary>
        /// <param name="animationEvent">Data associated with the animation event</param>
        public override void OnAnimationEvent(AnimationEvent animationEvent)
        {
            base.OnAnimationEvent(animationEvent);
            
            switch (animationEvent.stringParameter)
            {
                case "ProcessAttack":
                    Attack();
                    break;
                case "StopAttacking":
                    PlayerController.IsAttacking = false;
                    break;
                case "CanExit":
                    _canExit = true;
                    break;
            }
        }

        /// <summary>
        /// Performs the actual attack, checking for collision with enemies and dealing damage
        /// </summary>
        private void Attack()
        {
            if (Time.time - _lastAttackTime < AttackCooldown)
            {
                return;
            }
            
            AudioManager.Instance.PlaySwordSwingSound(PlayerController.transform.position);
            
            List<Collider2D> colliders = new();
            toolCollider.Overlap(contactFilter, colliders);

            var sword = GameManager.Instance.inventory.CurrentActiveItem.GetComponent<Sword>();
            
            foreach (var col in colliders)
            {
                if (!col.CompareTag("Enemy"))
                {
                    continue;
                }
                
                HitInfo hitInfo = new(sword.swordData.damage, PlayerController.transform.position);
                col.GetComponent<HealthController>().TakeDamage(hitInfo);
                _lastAttackTime = Time.time;
            }
        }

        /// <summary>
        /// Resets animations to idle
        /// </summary>
        public override void OnExit()
        {
            base.OnExit();
            ToolAnimator.CrossFade(IdleToolAnimHash, CrossFadeTime);
        }
    }
}
