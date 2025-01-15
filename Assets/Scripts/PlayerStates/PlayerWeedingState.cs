using UnityEngine;

namespace PlayerStates
{
    public class PlayerWeedingState : PlayerBaseState
    {
        public override string Name => "Weeding"; 

        public PlayerWeedingState(PlayerContoller playerContoller, Animator animator, Rigidbody2D rigidbody2D) : base(playerContoller, animator, rigidbody2D)
        {
        }

        public override void OnEnter()
        {
            base.OnEnter();
            Animator.CrossFade(WeedingAnimHash, CrossFadeTime);
        }
        
        public override void OnAnimationEvent(AnimationEvent animationEvent)
        {
            base.OnAnimationEvent(animationEvent);
            if (animationEvent.stringParameter == "StopWeeding")
            {
                PlayerContoller.IsWeeding = false;
            }
        }
    }
}