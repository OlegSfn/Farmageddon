using Managers;
using UnityEngine;
using UnityEngine.AI;

namespace Enemies.Slime.SlimeStates
{
    public class SlimeFriendlyState : SlimeBaseState
    {
        public override string Name => "Friendly";

        public SlimeFriendlyState(Slime slime, NavMeshAgent navMeshAgent, Animator animator) : base(slime, navMeshAgent, animator)
        {
        }
        
        public override void OnEnter()
        {
            base.OnEnter();
            Animator.CrossFade(WalkingAnimHash, CrossFadeTime);
            Vector2 randomAwayPoint = Random.insideUnitCircle.normalized * 50f;
            NavMeshAgent.SetDestination((Vector2)Slime.transform.position + randomAwayPoint);
        }
        
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