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

        private float _lastAttackTime;
        
        public SlimeAttackingState(Slime slime, NavMeshAgent navMeshAgent, Animator animator, Collider2D attackArea, 
            ContactFilter2D contactFilter2D) 
            : base(slime, navMeshAgent, animator)
        {
            _attackArea = attackArea;
            _contactFilter2D = contactFilter2D;
        }
        
        public override void OnEnter()
        {
            base.OnEnter();
            Animator.CrossFade(ChasingAnimHash, CrossFadeTime);
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
            // Casting null to UnityEngine.Object to check if Slime.Target is destroyed. 
            if (Slime.Target != (UnityEngine.Object)null)
            {
                NavMeshAgent.SetDestination(Slime.Target.position);
            }
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
            if (Time.time - _lastAttackTime < Slime.Data.attackCooldown)
            {
                return;
            }
            
            List<Collider2D> colliders = new List<Collider2D>();
            _attackArea.Overlap(_contactFilter2D, colliders);

            foreach (var collider in colliders)
            {
                HealthController healthController = collider.GetComponent<HealthController>();
                if (healthController != null && (collider.CompareTag("Player") || collider.CompareTag("Building")))
                {
                    HitInfo hitInfo = new HitInfo(Slime.Data.damage, Slime.transform.position);
                    healthController.TakeDamage(hitInfo);
                    _lastAttackTime = Time.time;
                    return;
                }
            }
        }
    }
}