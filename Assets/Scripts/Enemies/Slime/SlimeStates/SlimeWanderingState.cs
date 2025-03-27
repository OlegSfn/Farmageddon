using Managers;
using UnityEngine;
using UnityEngine.AI;

namespace Enemies.Slime.SlimeStates
{
    /// <summary>
    /// State when the slime is wandering around without a specific target
    /// </summary>
    public class SlimeWanderingState : SlimeBaseState
    {
        /// <summary>
        /// Maximum distance to search for a valid NavMesh point when generating random destinations
        /// </summary>
        private readonly float _errorRadius;

        /// <summary>
        /// Name of this state for state machine to check on equality
        /// </summary>
        public override string Name => "Wandering";

        /// <summary>
        /// Initializes a new wandering state with required components
        /// </summary>
        /// <param name="slime">The slime this state belongs to</param>
        /// <param name="navMeshAgent">Navigation agent for movement control</param>
        /// <param name="animator">Animator for visual feedback</param>
        /// <param name="errorRadius">How far to search for valid NavMesh points</param>
        public SlimeWanderingState(Slime slime, NavMeshAgent navMeshAgent, Animator animator, float errorRadius) : base(slime, navMeshAgent, animator)
        {
            _errorRadius = errorRadius;
        }

        /// <summary>
        /// Plays walking animation and sets initial random destination
        /// </summary>
        public override void OnEnter()
        {
            base.OnEnter();
            Animator.CrossFade(WalkingAnimHash, CrossFadeTime);
            NavMeshAgent.SetDestination(GetRandomPointToWalk());
        }

        /// <summary>
        /// Checks if destination has been reached and sets a new one if needed
        /// </summary>
        public override void OnUpdate()
        {
            base.OnUpdate();
            if (HasReachedDestination())
            {
                NavMeshAgent.SetDestination(GetRandomPointToWalk());
            }
        }

        /// <summary>
        /// Determines if the slime has reached its current destination
        /// </summary>
        /// <returns>True if the destination has been reached</returns>
        private bool HasReachedDestination()
        {
            return !NavMeshAgent.pathPending && NavMeshAgent.remainingDistance <= NavMeshAgent.stoppingDistance &&
                   (!NavMeshAgent.hasPath || NavMeshAgent.velocity.sqrMagnitude == 0f);
        }

        /// <summary>
        /// Generates a random point near the player to walk towards
        /// Ensures the point is on the NavMesh
        /// </summary>
        /// <returns>A valid position on the NavMesh</returns>
        private Vector2 GetRandomPointToWalk()
        {
            Vector2 playerPosition = GameManager.Instance.playerTransform.position;
            Vector2 randomPoint = new Vector2(playerPosition.x+Random.Range(-5f,5f), playerPosition.y+Random.Range(-5f,5f));

            NavMesh.SamplePosition(randomPoint, out NavMeshHit hit, _errorRadius, 1);
            return hit.position;
        }
    }
}