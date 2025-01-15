using UnityEngine;
using UnityEngine.AI;

namespace Enemies.Slime.SlimeStates
{
    public class SlimeDyingState : SlimeBaseState
    {
        public override string Name => "Dying";
        private readonly NavMeshAgent _navMeshAgent;
        
        public SlimeDyingState(Slime slime, NavMeshAgent navMeshAgent, Animator animator) : base(slime, navMeshAgent, animator)
        {
        }
        
        public override void OnEnter()
        {
            base.OnEnter();
            _navMeshAgent.isStopped = true;
            Animator.CrossFade(DyingAnimHash, CrossFadeTime);
        }

        public override void OnAnimationEvent(AnimationEvent animationEvent)
        {
            base.OnAnimationEvent(animationEvent);
            if (animationEvent.stringParameter == "Die")
            {
                Object.Destroy(Slime.gameObject);
            }
        }
    }
}