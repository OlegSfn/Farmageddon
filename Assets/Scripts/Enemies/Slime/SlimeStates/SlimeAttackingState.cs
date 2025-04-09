using System.Collections;
using System.Collections.Generic;
using Helpers;
using UnityEngine;
using UnityEngine.AI;

namespace Enemies.Slime.SlimeStates
{
    /// <summary>
    /// State when the slime is attacking a target - trying to get as close to enemy as it can
    /// </summary>
    public class SlimeAttackingState : SlimeBaseState
    {
        /// <summary>
        /// Name of this state for state machine to check on equality
        /// </summary>
        public override string Name => "Attacking";

        /// <summary>
        /// Collider used to detect attack hits
        /// </summary>
        private readonly Collider2D _attackArea;
        
        /// <summary>
        /// Filter to determine what colliders can be attacked
        /// </summary>
        private readonly ContactFilter2D _contactFilter2D;

        /// <summary>
        /// Timestamp of the last attack for cooldown calculation
        /// </summary>
        private float _lastAttackTime;
        
        /// <summary>
        /// Reference to the active walking coroutine for cleanup
        /// </summary>
        private Coroutine walkingToTargetCoroutine;
        
        /// <summary>
        /// Initializes a new attacking state with required components
        /// </summary>
        /// <param name="slime">The slime this state belongs to</param>
        /// <param name="navMeshAgent">Navigation agent for movement</param>
        /// <param name="animator">Animator for visual feedback</param>
        /// <param name="attackArea">Collider used for attack detection</param>
        /// <param name="contactFilter2D">Filter for attack targets</param>
        public SlimeAttackingState(Slime slime, NavMeshAgent navMeshAgent, Animator animator, Collider2D attackArea,
            ContactFilter2D contactFilter2D)
            : base(slime, navMeshAgent, animator)
        {
            _attackArea = attackArea;
            _contactFilter2D = contactFilter2D;
        }
        
        /// <summary>
        /// Starts animation and target following behavior
        /// </summary>
        public override void OnEnter()
        {
            base.OnEnter();
            Animator.CrossFade(ChasingAnimHash, CrossFadeTime);
            walkingToTargetCoroutine = Slime.StartCoroutine(WalkToTarget());
        }
        
        /// <summary>
        /// Cleans up the walking coroutine
        /// </summary>
        public override void OnExit()
        {
            base.OnExit();
            Slime.StopCoroutine(walkingToTargetCoroutine);
        }

        /// <summary>
        /// When the "Attack" event is received, the slime will try to perform an attack
        /// </summary>
        /// <param name="animationEvent">Data from the animation event</param>
        public override void OnAnimationEvent(AnimationEvent animationEvent)
        {
            base.OnAnimationEvent(animationEvent);
            if (animationEvent.stringParameter == "Attack")
            {
                Attack();
            }
        }

        /// <summary>
        /// Performs an attack if the cooldown has elapsed
        /// Deals damage to non-enemy entities with health controllers
        /// </summary>
        private void Attack()
        {
            if (Time.time - _lastAttackTime < Slime.Data.attackCooldown)
            {
                return;
            }
            
            Managers.AudioManager.Instance.PlaySlimeAttackSound(Slime.transform.position);
            
            List<Collider2D> colliders = new();
            _attackArea.Overlap(_contactFilter2D, colliders);

            foreach (var collider in colliders)
            {
                if (collider.CompareTag("Enemy") ||
                    !collider.TryGetComponent(out HealthController healthController))
                {
                    continue;
                }
                
                HitInfo hitInfo = new HitInfo(Slime.Data.damage, Slime.transform.position);
                healthController.TakeDamage(hitInfo);
                _lastAttackTime = Time.time;
            }
        }

        /// <summary>
        /// Coroutine that continuously updates the destination to follow the target
        /// </summary>
        /// <returns>Coroutine enumerator</returns>
        private IEnumerator WalkToTarget()
        {
            while (true)
            {
                // Casting null to UnityEngine.Object to check if Slime.Target is destroyed.
                if (Slime.Target != (UnityEngine.Object)null)
                {
                    NavMeshAgent.SetDestination(Slime.Target.position);
                }
                yield return new WaitForSeconds(0.1f);
            }
        }
    }
}