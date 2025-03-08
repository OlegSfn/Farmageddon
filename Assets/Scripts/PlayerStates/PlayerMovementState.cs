using UnityEngine;

namespace PlayerStates
{
    public class PlayerMovementState : PlayerBaseState
    {
        public override string Name => "Movement";
        
        private float _footstepTimer;
        private const float FootstepInterval = 0.3f;

        public PlayerMovementState(PlayerContoller playerContoller, Animator animator, Animator toolAnimator, Rigidbody2D rigidbody2D) 
            : base(playerContoller, animator, toolAnimator, rigidbody2D) { }

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
                _footstepTimer = 0f;
            }
            else
            {
                Animator.CrossFade(MovementAnimHash, CrossFadeTime);
                
                _footstepTimer -= Time.deltaTime;
                if (_footstepTimer <= 0f)
                {
                    Managers.AudioManager.Instance.PlayFootstepSound(PlayerContoller.transform.position);
                    _footstepTimer = FootstepInterval;
                }
            }
        }

        public override void OnFixedUpdate()
        {
            base.OnFixedUpdate();
            Rigidbody2D.linearVelocity = PlayerContoller.Speed * PlayerContoller.Input.normalized;
        }
    }
}