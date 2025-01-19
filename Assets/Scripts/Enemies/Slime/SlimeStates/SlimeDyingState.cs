using UnityEngine;
using UnityEngine.AI;

namespace Enemies.Slime.SlimeStates
{
    public class SlimeDyingState : SlimeBaseState
    {
        public override string Name => "Dying";
        
        public SlimeDyingState(Slime slime, NavMeshAgent navMeshAgent, Animator animator) : base(slime, navMeshAgent, animator)
        {
        }
        
        public override void OnEnter()
        {
            base.OnEnter();
            NavMeshAgent.isStopped = true;
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