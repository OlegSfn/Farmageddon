using Managers;
using UnityEngine;

namespace Player.PlayerStates
{
    /// <summary>
    /// Handles the player's movement state
    /// Manages player movement, idle transitions, and footstep sounds
    /// </summary>
    public class PlayerMovementState : PlayerBaseState
    {
        /// <summary>
        /// Name of this state for state machine to check on equality
        /// </summary>
        public override string Name => "Movement";
        
        /// <summary>
        /// Timer to control footstep sound frequency
        /// </summary>
        private float _footstepTimer;
        
        /// <summary>
        /// Time between footstep sounds in seconds
        /// </summary>
        private const float FootstepInterval = 0.3f;

        /// <summary>
        /// Constructor initializing the movement state with necessary components
        /// </summary>
        /// <param name="playerController">Reference to the player controller</param>
        /// <param name="animator">Reference to the player animator</param>
        /// <param name="toolAnimator">Reference to the tool animator</param>
        /// <param name="rigidbody2D">Reference to the player's rigidbody</param>
        public PlayerMovementState(PlayerController playerController, Animator animator, Animator toolAnimator, Rigidbody2D rigidbody2D)
            : base(playerController, animator, toolAnimator, rigidbody2D) { }

        /// <summary>
        /// Sets initial animation to idle
        /// </summary>
        public override void OnEnter()
        {
            base.OnEnter();
            Animator.CrossFade(IdleAnimHash, CrossFadeTime);
        }

        /// <summary>
        /// Updates animations and manages footstep sounds based on player input
        /// </summary>
        public override void OnUpdate()
        {
            base.OnUpdate();
            
            if (IsIdle())
            {
                HandleIdleState();
            }
            else if (PlayerController.CanMove)
            {
                HandleMovementState();
            }
        }

        /// <summary>
        /// Determines if the player is currently idle (not moving)
        /// </summary>
        /// <returns>True if player input is below movement threshold and game is not paused</returns>
        private bool IsIdle()
        {
            return PlayerController.Input.sqrMagnitude < 0.1f && !GameManager.Instance.IsPaused;
        }

        /// <summary>
        /// Handles the player's idle state
        /// </summary>
        private void HandleIdleState()
        {
            Animator.CrossFade(IdleAnimHash, CrossFadeTime);
            _footstepTimer = 0f;
        }

        /// <summary>
        /// Handles the player's movement state
        /// Updates animation and plays footstep sounds at appropriate intervals
        /// </summary>
        private void HandleMovementState()
        {
            Animator.CrossFade(MovementAnimHash, CrossFadeTime);
            
            _footstepTimer -= Time.deltaTime;
            if (!(_footstepTimer <= 0f))
            {
                return;
            }
            
            AudioManager.Instance.PlayFootstepSound(PlayerController.transform.position);
            _footstepTimer = FootstepInterval;
        }

        /// <summary>
        /// Updates the player's velocity based on input
        /// </summary>
        public override void OnFixedUpdate()
        {
            base.OnFixedUpdate();
            
            if (PlayerController.CanMove)
            {
                Rigidbody2D.linearVelocity = PlayerController.Speed * PlayerController.Input.normalized;
            }
            else
            {
                Rigidbody2D.linearVelocity = Vector2.zero;
            }
        }
    }
}