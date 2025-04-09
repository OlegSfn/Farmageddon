using Managers;
using UnityEngine;

namespace Player.PlayerStates
{
    /// <summary>
    /// Handles the player's watering state when using a watering can
    /// Manages watering animation and sound effects
    /// </summary>
    public class PlayerWateringState : PlayerBaseState
    {
        /// <summary>
        /// Name of this state for state machine to check on equality
        /// </summary>
        public override string Name => "Watering";

        /// <summary>
        /// Constructor initializing the watering state with necessary components
        /// </summary>
        /// <param name="playerController">Reference to the player controller</param>
        /// <param name="animator">Reference to the player animator</param>
        /// <param name="toolAnimator">Reference to the tool animator</param>
        /// <param name="rigidbody2D">Reference to the player's rigidbody</param>
        public PlayerWateringState(PlayerController playerController, Animator animator, Animator toolAnimator, Rigidbody2D rigidbody2D) : base(playerController, animator, toolAnimator, rigidbody2D)
        {
        }

        /// <summary>
        /// Stops player movement, plays watering animation and sound
        /// </summary>
        public override void OnEnter()
        {
            base.OnEnter();
            Rigidbody2D.linearVelocity = Vector2.zero;
            
            Animator.CrossFade(WateringAnimHash, CrossFadeTime);
            ToolAnimator.CrossFade(UseToolAnimHash, CrossFadeTime);
            
            AudioManager.Instance.PlayWateringCanSound(PlayerController.transform.position);
        }
        
        /// <summary>
        /// When the "StopWatering" event is received, the player is no longer watering
        /// </summary>
        /// <param name="animationEvent">Data associated with the animation event</param>
        public override void OnAnimationEvent(AnimationEvent animationEvent)
        {
            base.OnAnimationEvent(animationEvent);
            
            if (animationEvent.stringParameter == "StopWatering")
            {
                PlayerController.IsWatering = false;
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