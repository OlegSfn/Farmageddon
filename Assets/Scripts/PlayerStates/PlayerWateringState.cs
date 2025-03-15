using Managers;
using UnityEngine;

namespace PlayerStates
{
    public class PlayerWateringState : PlayerBaseState
    {
        public override string Name => "Watering";

        public PlayerWateringState(PlayerContoller playerContoller, Animator animator, Animator toolAnimator, Rigidbody2D rigidbody2D) : base(playerContoller, animator, toolAnimator, rigidbody2D)
        {
        }

        public override void OnEnter()
        {
            base.OnEnter();
            Rigidbody2D.linearVelocity = Vector2.zero;
            Animator.CrossFade(WateringAnimHash, CrossFadeTime);
            ToolAnimator.CrossFade(UseToolAnimHash, CrossFadeTime);
            
            AudioManager.Instance.PlayWateringCanSound(PlayerContoller.transform.position);
        }
        
        public override void OnAnimationEvent(AnimationEvent animationEvent)
        {
            base.OnAnimationEvent(animationEvent);
            if (animationEvent.stringParameter == "StopWatering")
            {
                PlayerContoller.IsWatering = false;
            }
        }

        public override void OnExit()
        {
            base.OnExit();
            ToolAnimator.CrossFade(IdleToolAnimHash, CrossFadeTime);
        }
    }
}