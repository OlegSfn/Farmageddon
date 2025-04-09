using System.Collections;
using Building.Concrete.Scarecrow;
using UnityEngine;
using UnityEngine.AI;

namespace Enemies.Slime.SlimeStates
{
    /// <summary>
    /// State when the slime is frightened by a scary object and flees from it
    /// </summary>
    public class SlimeScaredState : SlimeBaseState
    {
        /// <summary>
        /// Name of this state for state machine to check on equality
        /// </summary>
        public override string Name => "Scared";
        
        /// <summary>
        /// Reference to the active fleeing coroutine for cleanup
        /// </summary>
        private Coroutine walkFromScariestSourceCoroutine;
        
        /// <summary>
        /// Initializes a new scared state with required components
        /// </summary>
        /// <param name="slime">The slime this state belongs to</param>
        /// <param name="navMeshAgent">Navigation agent for movement control</param>
        /// <param name="animator">Animator for visual feedback</param>
        public SlimeScaredState(Slime slime, NavMeshAgent navMeshAgent, Animator animator) : base(slime, navMeshAgent, animator)
        {
        }
        
        /// <summary>
        /// Plays walking animation and starts fleeing behavior
        /// </summary>
        public override void OnEnter()
        {
            base.OnEnter();
            Animator.CrossFade(WalkingAnimHash, CrossFadeTime);
            walkFromScariestSourceCoroutine = Slime.StartCoroutine(WalkFromScariestSource());
        }
        
        /// <summary>
        /// Cleans up the fleeing coroutine
        /// </summary>
        public override void OnExit()
        {
            base.OnExit();
            Slime.StopCoroutine(walkFromScariestSourceCoroutine);
        }

        /// <summary>
        /// Coroutine that continuously updates the destination to flee from the scariest source
        /// If no scary source is found, moves in a random direction
        /// </summary>
        /// <returns>Coroutine enumerator</returns>
        private IEnumerator WalkFromScariestSource()
        {
            while (true)
            {
                IScary mostScarySource = Slime.GetStrongestScareSource();
                if (mostScarySource is not null)
                {
                    // Calculate a point in the opposite direction from the scary source
                    Vector2 pointFromScare = 50 * (Slime.transform.position - mostScarySource.GetTransform().position)
                        .normalized;
                    NavMeshAgent.SetDestination(pointFromScare);
                }
                else
                {
                    // If no scary source found, just move in a random direction
                    Vector2 randomAwayPoint = Random.insideUnitCircle.normalized * 50f;
                    NavMeshAgent.SetDestination((Vector2)Slime.transform.position + randomAwayPoint);
                }

                yield return new WaitForSeconds(0.1f);
            }
        }
    }
}