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

        private bool _canExit;
        
        public PlayerAttackingState(PlayerContoller playerContoller, Animator animator, Rigidbody2D rigidbody2D, Collider2D toolCollider) : base(playerContoller, animator, rigidbody2D)
        {
            this.toolCollider = toolCollider;
            contactFilter = new ContactFilter2D();
            contactFilter.NoFilter();
        }
        
        public override void OnEnter()
        {
            base.OnEnter();
            _canExit = false;
            Rigidbody2D.linearVelocity = Vector2.zero;
            Animator.CrossFade(AttackingAnimHash, CrossFadeTime);
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
            if (PlayerContoller.Input != Vector2.zero && _canExit)
            {
                PlayerContoller.IsAttacking = false;
            }
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
            } else if (animationEvent.stringParameter == "CanExit")
            {
                _canExit = true;
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
                    HitInfo hitInfo = new(sword.swordData.damage, PlayerContoller.transform.position);
                    col.GetComponent<HealthController>().TakeDamage(hitInfo);
                    _lastAttackTime = Time.time;
                }
            }
        }
    }
}