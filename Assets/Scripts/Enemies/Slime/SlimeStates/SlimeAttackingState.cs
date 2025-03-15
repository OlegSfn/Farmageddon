using System.Collections;
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
        private Coroutine walkingToTargetCoroutine;
        
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
            walkingToTargetCoroutine = Slime.StartCoroutine(WalkToTarget());
        }
        
        public override void OnExit()
        {
            base.OnExit();
            Slime.StopCoroutine(walkingToTargetCoroutine);
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
            
            Managers.AudioManager.Instance.PlaySlimeAttackSound(Slime.transform.position);
            
            List<Collider2D> colliders = new List<Collider2D>();
            _attackArea.Overlap(_contactFilter2D, colliders);

            foreach (var collider in colliders)
            {
                HealthController healthController = collider.GetComponent<HealthController>();
                if (healthController != null && (!collider.CompareTag("Enemy")))
                {
                    HitInfo hitInfo = new HitInfo(Slime.Data.damage, Slime.transform.position);
                    healthController.TakeDamage(hitInfo);
                    _lastAttackTime = Time.time;
                    return;
                }
            }
        }

        private IEnumerator WalkToTarget()
        {
            // Casting null to UnityEngine.Object to check if Slime.Target is destroyed. 
            while (Slime.Target != (UnityEngine.Object)null)
            {
                NavMeshAgent.SetDestination(Slime.Target.position);
                yield return new WaitForSeconds(0.1f);
            }
        }
    }
}