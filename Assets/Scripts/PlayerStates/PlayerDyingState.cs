using UnityEngine;

namespace PlayerStates
{
    /// <summary>
    /// Handles the player's death state
    /// Triggers the death animation and destroys the player object when complete
    /// </summary>
    public class PlayerDyingState : PlayerBaseState
    {
        /// <summary>
        /// Name of this state for state machine to check on equality
        /// </summary>
        public override string Name => "Dying";

        /// <summary>
        /// Constructor initializing the dying state with necessary components
        /// </summary>
        /// <param name="playerContoller">Reference to the player controller</param>
        /// <param name="animator">Reference to the player animator</param>
        /// <param name="toolAnimator">Reference to the tool animator</param>
        /// <param name="rigidbody2D">Reference to the player's rigidbody</param>
        public PlayerDyingState(PlayerContoller playerContoller, Animator animator, Animator toolAnimator, Rigidbody2D rigidbody2D) : base(playerContoller, animator, toolAnimator, rigidbody2D)
        {
        }
        
        /// <summary>
        /// Starts the death animation
        /// </summary>
        public override void OnEnter()
        {
            base.OnEnter();
            Animator.CrossFade(DyingAnimHash, CrossFadeTime);
        }

        /// <summary>
        /// When the "Die" event is received, destroys the game object
        /// </summary>
        /// <param name="animationEvent">Data associated with the animation event</param>
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