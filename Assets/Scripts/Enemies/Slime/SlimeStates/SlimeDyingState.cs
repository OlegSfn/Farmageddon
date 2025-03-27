using Managers;
using UnityEngine;
using UnityEngine.AI;

namespace Enemies.Slime.SlimeStates
{
    /// <summary>
    /// State representing the slime's death sequence
    /// </summary>
    public class SlimeDyingState : SlimeBaseState
    {
        /// <summary>
        /// Name of this state for state machine to check on equality
        /// </summary>
        public override string Name => "Dying";
        
        /// <summary>
        /// Initializes a new dying state with required components
        /// </summary>
        /// <param name="slime">The slime this state belongs to</param>
        /// <param name="navMeshAgent">Navigation agent for movement control</param>
        /// <param name="animator">Animator for visual feedback</param>
        public SlimeDyingState(Slime slime, NavMeshAgent navMeshAgent, Animator animator) : base(slime, navMeshAgent, animator)
        {
        }
        
        /// <summary>
        /// Stops movement, plays death animation and sound
        /// </summary>
        public override void OnEnter()
        {
            base.OnEnter();
            NavMeshAgent.isStopped = true;
            Animator.CrossFade(DyingAnimHash, CrossFadeTime);
            
            AudioManager.Instance.PlaySlimeDeathSound(Slime.transform.position);
        }

        /// <summary>
        /// When the "Die" event is received, destroys the game object
        /// </summary>
        /// <param name="animationEvent">Data from the animation event</param>
        public override void OnAnimationEvent(AnimationEvent animationEvent)
        {
            base.OnAnimationEvent(animationEvent);
            if (animationEvent.stringParameter == "Die")
            {
                Slime.NeedRewardForDying = true;
                Object.Destroy(Slime.gameObject);
            }
        }
    }
}