using Managers;
using UnityEngine;
using UnityEngine.AI;

namespace Enemies.Slime.SlimeStates
{
    public class SlimeWanderingState : SlimeBaseState
    {
        private readonly float _errorRadius;

        public override string Name => "Wandering";

        public SlimeWanderingState(Slime slime, NavMeshAgent navMeshAgent, Animator animator, float errorRadius) : base(slime, navMeshAgent, animator)
        {
            _errorRadius = errorRadius;
        }

        public override void OnEnter()
        {
            base.OnEnter();
            Animator.CrossFade(WalkingAnimHash, CrossFadeTime);
            NavMeshAgent.SetDestination(GetRandomPointToWalk());
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
            if (HasReachedDestination())
            {
                NavMeshAgent.SetDestination(GetRandomPointToWalk());
            }
        }

        private bool HasReachedDestination()
        {
            return !NavMeshAgent.pathPending && NavMeshAgent.remainingDistance <= NavMeshAgent.stoppingDistance && 
                   (!NavMeshAgent.hasPath || NavMeshAgent.velocity.sqrMagnitude == 0f);
        }

        private Vector2 GetRandomPointToWalk()
        {
            Vector2 playerPosition = GameManager.Instance.playerTransform.position;
            Vector2 randomPoint = new Vector2(playerPosition.x+Random.Range(-5f,5f), playerPosition.y+Random.Range(-5f,5f));

            NavMesh.SamplePosition(randomPoint, out NavMeshHit hit, _errorRadius, 1);
            return hit.position;
        }
    }
}