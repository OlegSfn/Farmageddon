using Managers;
using UnityEngine;

namespace Player.PlayerStates
{
    /// <summary>
    /// Handles the player's weeding state when using a hoe or similar tool
    /// Manages weeding animation and sound effects for farming activities
    /// </summary>
    public class PlayerWeedingState : PlayerBaseState
    {
        /// <summary>
        /// Name of this state for state machine to check on equality
        /// </summary>
        public override string Name => "Weeding";

        /// <summary>
        /// Constructor initializing the weeding state with necessary components
        /// </summary>
        /// <param name="playerController">Reference to the player controller</param>
        /// <param name="animator">Reference to the player animator</param>
        /// <param name="toolAnimator">Reference to the tool animator</param>
        /// <param name="rigidbody2D">Reference to the player's rigidbody</param>
        public PlayerWeedingState(PlayerController playerController, Animator animator, Animator toolAnimator, Rigidbody2D rigidbody2D) : base(playerController, animator, toolAnimator, rigidbody2D)
        {
        }

        /// <summary>
        /// Stops player movement, plays weeding animation and soil digging sound
        /// </summary>
        public override void OnEnter()
        {
            base.OnEnter();
            Rigidbody2D.linearVelocity = Vector2.zero;
            
            Animator.CrossFade(WeedingAnimHash, CrossFadeTime);
            ToolAnimator.CrossFade(UseToolAnimHash, CrossFadeTime);

            AudioManager.Instance.PlayPluggingDirtSound(PlayerController.transform.position);
        }
        
        /// <summary>
        /// When the "StopWeeding" event is received, the player is no longer weeding
        /// </summary>
        /// <param name="animationEvent">Data associated with the animation event</param>
        public override void OnAnimationEvent(AnimationEvent animationEvent)
        {
            base.OnAnimationEvent(animationEvent);
            
            if (animationEvent.stringParameter == "StopWeeding")
            {
                PlayerController.IsWeeding = false;
            }
        }

        /// <summary>
        /// Resets tool animation to idle
        /// </summary>
        public override void OnExit()
        {
            base.OnExit();
            ToolAnimator.CrossFade(IdleToolAnimHash, CrossFadeTime);
        }
    }
}