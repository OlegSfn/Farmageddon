using System.Collections.Generic;
using UnityEngine;

namespace PlayerStates
{
    public class PlayerAttackingState : PlayerBaseState
    {
        public override string Name => "Attacking";

        private Collider2D toolCollider;
        private readonly ContactFilter2D contactFilter;
        
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
                List<Collider2D> colliders = new List<Collider2D>();
                toolCollider.Overlap(contactFilter, colliders);
                
                foreach (var col in colliders)
                {
                    if (col.CompareTag("Enemy"))
                    {
                        col.GetComponent<HealthController>().TakeDamage(5);
                    }
                }
            }
            else if (animationEvent.stringParameter == "StopAttacking")
            {
                PlayerContoller.IsAttacking = false;
            }
        }
    }
}