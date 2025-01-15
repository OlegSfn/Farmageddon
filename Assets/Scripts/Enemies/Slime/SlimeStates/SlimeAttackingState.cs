using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Enemies.Slime.SlimeStates
{
    public class SlimeAttackingState : SlimeBaseState
    {
        public override string Name => "Attacking";

        private readonly Collider2D _attackArea;
        private readonly ContactFilter2D _contactFilter2D;
        private readonly int _damage;

        private const float AttackCooldown = 0.01f;
        private float _lastAttackTime;
        
        public SlimeAttackingState(Slime slime, NavMeshAgent navMeshAgent, Animator animator, Collider2D attackArea, 
            ContactFilter2D contactFilter2D, int damage) 
            : base(slime, navMeshAgent, animator)
        {
            _attackArea = attackArea;
            _contactFilter2D = contactFilter2D;
            _damage = damage;
        }
        
        public override void OnEnter()
        {
            base.OnEnter();
            Animator.CrossFade(ChasingAnimHash, CrossFadeTime);
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
            NavMeshAgent.SetDestination(Slime.Target.position);
        }

        public override void OnAnimationEvent(AnimationEvent animationEvent)
        {
            base.OnAnimationEvent(animationEvent);
            if (animationEvent.stringParameter == "Attack")
            {
                Attack();
            }
        }

        private void Attack()
        {
            if (Time.time - _lastAttackTime < AttackCooldown)
            {
                return;
            }
            
            List<Collider2D> colliders = new List<Collider2D>();
            _attackArea.Overlap(_contactFilter2D, colliders);

            foreach (var collider in colliders)
            {
                Health health = collider.GetComponent<Health>();
                if (health != null && collider.CompareTag("Player") || collider.CompareTag("Building"))
                {
                    health.TakeDamage(_damage);
                    _lastAttackTime = Time.time;
                    return;
                }
            }
        }
    }
}