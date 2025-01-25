using UnityEngine;

namespace PlayerStates
{
    public class PlayerDyingState : PlayerBaseState
    {
        public override string Name => "Dying";

        public PlayerDyingState(PlayerContoller playerContoller, Animator animator, Rigidbody2D rigidbody2D) : base(playerContoller, animator, rigidbody2D)
        {
        }
        
        public override void OnEnter()
        {
            base.OnEnter();
            Animator.CrossFade(DyingAnimHash, CrossFadeTime);
        }

        public override void OnAnimationEvent(AnimationEvent animationEvent)
        {
            base.OnAnimationEvent(animationEvent);
            if (animationEvent.stringParameter == "Die")
            {
                Object.Destroy(PlayerContoller.gameObject);
            }
        }
        
    }
}