using Helpers;
using Managers;
using UnityEngine;
using UnityEngine.AI;

namespace Enemies.Slime.SlimeStates
{
    /// <summary>
    /// State when the slime is taking damage from a hit, including knock back and visual feedback
    /// </summary>
    public class SlimeTakingDamageState : SlimeBaseState
    {
        /// <summary>
        /// Name of this state for state machine to check on equality
        /// </summary>
        public override string Name => "TakingDamage";

        /// <summary>
        /// Target position for knock back effect
        /// </summary>
        private Vector2 knockBackPosition;
        
        /// <summary>
        /// How far the slime gets knocked back when hit
        /// </summary>
        private const float KnockBackDistance = 1f;
        
        /// <summary>
        /// How fast the slime moves during knock back
        /// </summary>
        private const float KnockBackSpeed = 7f;
        
        /// <summary>
        /// Animation parameter hashes for hit direction
        /// </summary>
        private static readonly int HitX = Animator.StringToHash("HitX");
        private static readonly int HitY = Animator.StringToHash("HitY");
        
        /// <summary>
        /// Information about the hit that caused damage
        /// </summary>
        public HitInfo HitInfo { get; set; }

        /// <summary>
        /// Initializes a new taking damage state with required components
        /// </summary>
        /// <param name="slime">The slime this state belongs to</param>
        /// <param name="navMeshAgent">Navigation agent for movement control</param>
        /// <param name="animator">Animator for visual feedback</param>
        public SlimeTakingDamageState(Slime slime, NavMeshAgent navMeshAgent, Animator animator) : base(slime, navMeshAgent, animator)
        {
        }
        
        /// <summary>
        /// Sets up animation parameters, stops navigation, calculates knock back, and plays sound
        /// </summary>
        public override void OnEnter()
        {
            base.OnEnter();
            
            Vector2 selfPosition = Slime.transform.position;
            Animator.SetFloat(HitX, HitInfo.HitPoint.x - selfPosition.x);
            Animator.SetFloat(HitY, HitInfo.HitPoint.y - selfPosition.y);
            Animator.CrossFade(TakingDamageAnimHash, CrossFadeTime);
            
            NavMeshAgent.isStopped = true;
            
            Vector3 knockBackDirection = ((Vector2)Slime.transform.position - HitInfo.HitPoint).normalized;
            knockBackPosition = Slime.transform.position + KnockBackDistance*knockBackDirection;
            
            AudioManager.Instance.PlaySlimeTakingDamageSound(Slime.transform.position);
        }

        /// <summary>
        /// Applies knock back movement
        /// </summary>
        public override void OnUpdate()
        {
            base.OnUpdate();
            Slime.transform.position = Vector3.MoveTowards(Slime.transform.position, knockBackPosition, KnockBackSpeed * Time.deltaTime);
        }

        /// <summary>
        /// When the "EndTakingDamage" event is received, the slime is no longer taking damage
        /// </summary>
        /// <param name="animationEvent">Data from the animation event</param>
        public override void OnAnimationEvent(AnimationEvent animationEvent)
        {
            base.OnAnimationEvent(animationEvent);
            if (animationEvent.stringParameter != "EndTakingDamage")
            {
                return;
            }
            
            Slime.isTakingDamage = false;
            NavMeshAgent.isStopped = false;
        }
    }
}