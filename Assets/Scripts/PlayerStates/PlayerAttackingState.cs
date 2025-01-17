using Managers;
using UnityEngine;

namespace PlayerStates
{
    public class PlayerAttackingState : PlayerBaseState
    {
        public override string Name => "Attacking";


        public PlayerAttackingState(PlayerContoller playerContoller, Animator animator, Rigidbody2D rigidbody2D) : base(playerContoller, animator, rigidbody2D)
        {
        }
        
        public override void OnEnter()
        {
            base.OnEnter();
            Rigidbody2D.linearVelocity = Vector2.zero;
            Animator.CrossFade(AttackingAnimHash, CrossFadeTime);
        }
        
        public override void OnAnimationEvent(AnimationEvent animationEvent)
        {
            base.OnAnimationEvent(animationEvent);
            if (animationEvent.stringParameter == "ProcessAttack")
            {
                //TODO: attack enemies. (get damage data from used item)
                //TODO: setup collider in animator
            }
            else if (animationEvent.stringParameter == "StopAttacking")
            {
                PlayerContoller.IsAttacking = false;
            }
        }
    }
}