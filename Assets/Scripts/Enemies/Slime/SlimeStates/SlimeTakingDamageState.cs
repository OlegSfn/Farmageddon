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
        private static readonly int HitX = Animator.StringToHash("HitX");
        private static readonly int HitY = Animator.StringToHash("HitY");
        
        public HitInfo HitInfo { get; set; }

        public SlimeTakingDamageState(Slime slime, NavMeshAgent navMeshAgent, Animator animator) : base(slime, navMeshAgent, animator)
        {
        }
        
        public override void OnEnter()
        {
            base.OnEnter();
            
            Vector2 selfPosition = Slime.transform.position;
            Animator.SetFloat(HitX, HitInfo.HitPoint.x - selfPosition.x);
            Animator.SetFloat(HitY, HitInfo.HitPoint.y - selfPosition.y);
            Animator.CrossFade(TakingDamageAnimHash, CrossFadeTime);
            
            NavMeshAgent.isStopped = true;
            
            Vector3 knockBackDirection = ((Vector2)Slime.transform.position - HitInfo.HitPoint).normalized;
            knockBackPosition = Slime.transform.position + KnockBackDistance*knockBackDirection;
            
            AudioManager.Instance.PlaySlimeTakingDamageSound(Slime.transform.position);
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