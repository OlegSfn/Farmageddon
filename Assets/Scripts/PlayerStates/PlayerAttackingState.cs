using System.Collections.Generic;
using Items;
using Managers;
using UnityEngine;

namespace PlayerStates
{
    public class PlayerAttackingState : PlayerBaseState
    {
        public override string Name => "Attacking";

        private readonly Collider2D toolCollider;
        private readonly ContactFilter2D contactFilter;
        
        private const float AttackCooldown = 0.01f;
        private float _lastAttackTime;
        
        public PlayerAttackingState(PlayerContoller playerContoller, Animator animator, Rigidbody2D rigidbody2D, Collider2D toolCollider) : base(playerContoller, animator, rigidbody2D)
        {
            this.toolCollider = toolCollider;
            contactFilter = new ContactFilter2D();
            contactFilter.NoFilter();
        }
        
        public override void OnEnter()
        {
            base.OnEnter();
            Rigidbody2D.linearVelocity = Vector2.zero;
            Animator.CrossFade(AttackingAnimHash, CrossFadeTime);
        }
        
        public override void OnAnimationEvent(AnimationEvent animationEvent)
        {
            base.OnAnimationEvent(animationEvent);
            if (animationEvent.stringParameter == "ProcessAttack")
            {
                Attack();
            }
            else if (animationEvent.stringParameter == "StopAttacking")
            {
                PlayerContoller.IsAttacking = false;
            }
        }

        private void Attack()
        {
            if (Time.time - _lastAttackTime < AttackCooldown)
            {
                return;
            }
            
            List<Collider2D> colliders = new();
            toolCollider.Overlap(contactFilter, colliders);

            var sword = GameManager.Instance.inventory.CurrentActiveItem.GetComponent<Sword>();
            foreach (var col in colliders)
            {
                if (col.CompareTag("Enemy"))
                {
                    col.GetComponent<HealthController>().TakeDamage(sword.swordData.damage);
                    _lastAttackTime = Time.time;
                }
            }
        }
    }
}