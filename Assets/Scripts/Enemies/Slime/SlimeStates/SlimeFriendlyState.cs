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
            float t = Random.Range(0f, 2 * Mathf.PI);
            Vector2 randomAwayPoint = 50 * new Vector2(Mathf.Cos(t), Mathf.Sin(t));
            NavMeshAgent.SetDestination(randomAwayPoint);
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