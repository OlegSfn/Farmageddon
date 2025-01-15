using UnityEngine;

namespace PlayerStates
{
    public class PlayerMovementState : PlayerBaseState
    {
        public override string Name => "Movement";

        public PlayerMovementState(PlayerContoller playerContoller, Animator animator, Rigidbody2D rigidbody2D) 
            : base(playerContoller, animator, rigidbody2D) { }

        public override void OnEnter()
        {
            base.OnEnter();
            Animator.CrossFade(IdleAnimHash, CrossFadeTime);
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
            if (PlayerContoller.Input.sqrMagnitude < 0.1f)
            {
                Animator.CrossFade(IdleAnimHash, CrossFadeTime);
            }
            else
            {
                Animator.CrossFade(MovementAnimHash, CrossFadeTime);
            }
        }

        public override void OnFixedUpdate()
        {
            base.OnFixedUpdate();
            Rigidbody2D.linearVelocity = PlayerContoller.Speed * PlayerContoller.Input.normalized;
        }
    }
}