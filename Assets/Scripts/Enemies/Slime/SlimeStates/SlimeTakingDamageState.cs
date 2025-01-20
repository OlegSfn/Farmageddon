using Managers;
using UnityEngine;
using UnityEngine.AI;

namespace Enemies.Slime.SlimeStates
{
    public class SlimeTakingDamageState : SlimeBaseState
    {
        public override string Name => "TakingDamage";

        private Vector2 knockBackPosition;
        
        private const float KnockBackDistance = 1f;
        private const float KnockBackSpeed = 7f;

        public SlimeTakingDamageState(Slime slime, NavMeshAgent navMeshAgent, Animator animator) : base(slime, navMeshAgent, animator)
        {
        }
        
        public override void OnEnter()
        {
            base.OnEnter();
            Animator.CrossFade(TakingDamageAnimHash, CrossFadeTime);
            NavMeshAgent.isStopped = true;
            Vector3 knockBackDirection = (Slime.transform.position - GameManager.Instance.playerTransform.position).normalized;
            knockBackPosition = Slime.transform.position + KnockBackDistance*knockBackDirection;
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
            
            Slime.transform.position = Vector3.MoveTowards(Slime.transform.position, knockBackPosition, KnockBackSpeed * Time.deltaTime);

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