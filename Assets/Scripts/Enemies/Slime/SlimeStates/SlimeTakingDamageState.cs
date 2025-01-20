using UnityEngine;
using UnityEngine.AI;

namespace Enemies.Slime.SlimeStates
{
    public class SlimeTakingDamageState : SlimeBaseState
    {
        public override string Name => "TakingDamage";

        public SlimeTakingDamageState(Slime slime, NavMeshAgent navMeshAgent, Animator animator) : base(slime, navMeshAgent, animator)
        {
        }
        
        public override void OnEnter()
        {
            base.OnEnter();
            Animator.CrossFade(TakingDamageAnimHash, CrossFadeTime);
            NavMeshAgent.isStopped = true;
        }

        public override void OnAnimationEvent(AnimationEvent animationEvent)
        {
            base.OnAnimationEvent(animationEvent);
            if (animationEvent.stringParameter == "EndTakingDamage")
            {
                Slime.isTakingDamage = false;
                NavMeshAgent.isStopped = false;
            }
        }
    }
}