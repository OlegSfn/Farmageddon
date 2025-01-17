using UnityEngine;

namespace PlayerStates
{
    public class PlayerWateringState : PlayerBaseState
    {
        public override string Name => "Watering";

        public PlayerWateringState(PlayerContoller playerContoller, Animator animator, Rigidbody2D rigidbody2D) : base(playerContoller, animator, rigidbody2D)
        {
        }

        public override void OnEnter()
        {
            base.OnEnter();
            Rigidbody2D.linearVelocity = Vector2.zero;
            Animator.CrossFade(WateringAnimHash, CrossFadeTime);
        }
        
        public override void OnAnimationEvent(AnimationEvent animationEvent)
        {
            base.OnAnimationEvent(animationEvent);
            if (animationEvent.stringParameter == "StopWatering")
            {
                PlayerContoller.IsWatering = false;
            }
        }
    }
}