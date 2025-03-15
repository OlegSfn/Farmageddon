using System.Collections;
using System.Linq;
using Managers;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.PlayerLoop;

namespace Enemies.Slime.SlimeStates
{
    public class SlimeScaredState : SlimeBaseState
    {
        public override string Name => "Scared";
        
        private Coroutine walkFromScariestSourceCoroutine; 
        
        public SlimeScaredState(Slime slime, NavMeshAgent navMeshAgent, Animator animator) : base(slime, navMeshAgent, animator)
        {
        }
        
        public override void OnEnter()
        {
            base.OnEnter();
            Animator.CrossFade(WalkingAnimHash, CrossFadeTime);
            walkFromScariestSourceCoroutine = Slime.StartCoroutine(WalkFromScariestSource());
        }
        
        public override void OnExit()
        {
            base.OnExit();
            Slime.StopCoroutine(walkFromScariestSourceCoroutine);
        }

        private IEnumerator WalkFromScariestSource()
        {
            while (true)
            {
                IScary mostScarySource = Slime.GetStrongestScareSource();
                if (mostScarySource is not null)
                {
                    Vector2 pointFromScare = 50 * (Slime.transform.position - mostScarySource.GetTransform().position)
                        .normalized;
                    NavMeshAgent.SetDestination(pointFromScare);
                }
                else
                {
                    Vector2 randomAwayPoint = Random.insideUnitCircle.normalized * 50f;
                    NavMeshAgent.SetDestination((Vector2)Slime.transform.position + randomAwayPoint);
                }

                yield return new WaitForSeconds(0.1f);
            }
        }
    }
}