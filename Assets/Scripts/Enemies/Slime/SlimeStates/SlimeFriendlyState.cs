using Managers;
using UnityEngine;
using UnityEngine.AI;

namespace Enemies.Slime.SlimeStates
{
    /// <summary>
    /// State representing the slime's friendly behavior during daytime
    /// </summary>
    public class SlimeFriendlyState : SlimeBaseState
    {
        /// <summary>
        /// Name of this state for state machine to check on equality
        /// </summary>
        public override string Name => "Friendly";

        /// <summary>
        /// Initializes a new friendly state with required components
        /// </summary>
        /// <param name="slime">The slime this state belongs to</param>
        /// <param name="navMeshAgent">Navigation agent for movement control</param>
        /// <param name="animator">Animator for visual feedback</param>
        public SlimeFriendlyState(Slime slime, NavMeshAgent navMeshAgent, Animator animator) : base(slime, navMeshAgent, animator)
        {
        }
        
        /// <summary>
        /// Plays walking animation and sets a random destination away from current position
        /// </summary>
        public override void OnEnter()
        {
            base.OnEnter();
            Animator.CrossFade(WalkingAnimHash, CrossFadeTime);
            Vector2 randomAwayPoint = Random.insideUnitCircle.normalized * 50f;
            NavMeshAgent.SetDestination((Vector2)Slime.transform.position + randomAwayPoint);
        }
        
        /// <summary>
        /// Destroys the slime if it gets too far from the player
        /// </summary>
        public override void OnUpdate()
        {
            base.OnUpdate();
            if (Vector2.Distance(Slime.transform.position, GameManager.Instance.playerTransform.position) > 10)
            {
                Object.Destroy(Slime.gameObject);
            }
        }
    }
}