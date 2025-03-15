using Managers;
using UnityEngine;

namespace PlayerStates
{
    public class PlayerWeedingState : PlayerBaseState
    {
        public override string Name => "Weeding"; 

        public PlayerWeedingState(PlayerContoller playerContoller, Animator animator, Animator toolAnimator, Rigidbody2D rigidbody2D) : base(playerContoller, animator, toolAnimator, rigidbody2D)
        {
        }

        public override void OnEnter()
        {
            base.OnEnter();
            Rigidbody2D.linearVelocity = Vector2.zero;
            Animator.CrossFade(WeedingAnimHash, CrossFadeTime);
            ToolAnimator.CrossFade(UseToolAnimHash, CrossFadeTime);

            AudioManager.Instance.PlayPluggingDirtSound(PlayerContoller.transform.position);
        }
        
        public override void OnAnimationEvent(AnimationEvent animationEvent)
        {
            base.OnAnimationEvent(animationEvent);
            if (animationEvent.stringParameter == "StopWeeding")
            {
                PlayerContoller.IsWeeding = false;
            }
        }

        public override void OnExit()
        {
            base.OnExit();
            ToolAnimator.CrossFade(IdleToolAnimHash, CrossFadeTime);
        }
    }
}