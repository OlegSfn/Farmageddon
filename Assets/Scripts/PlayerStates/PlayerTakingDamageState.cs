using UnityEngine;

namespace PlayerStates
{
    /// <summary>
    /// Handles the player's state when taking damage from enemies or hazards
    /// Manages damage animation, knockback effect, and hit reaction
    /// </summary>
    public class PlayerTakingDamageState : PlayerBaseState
    {
        /// <summary>
        /// Name of this state for state machine to check on equality
        /// </summary>
        public override string Name => "TakingDamage";
        
        /// <summary>
        /// Target position where player will be knocked back to
        /// </summary>
        private Vector2 knockBackPosition;
        
        /// <summary>
        /// Distance player gets knocked back when hit
        /// </summary>
        private const float KnockBackDistance = 1f;
        
        /// <summary>
        /// Speed at which player moves during knock back
        /// </summary>
        private const float KnockBackSpeed = 7f;
        
        /// <summary>
        /// Animation parameter hashes for hit direction
        /// </summary>
        private static readonly int HitX = Animator.StringToHash("HitX");
        private static readonly int HitY = Animator.StringToHash("HitY");

        /// <summary>
        /// Information about the hit that caused damage
        /// </summary>
        public HitInfo HitInfo { get; set; }
        
        /// <summary>
        /// Constructor initializing the damage state with necessary components
        /// </summary>
        /// <param name="playerContoller">Reference to the player controller</param>
        /// <param name="animator">Reference to the player animator</param>
        /// <param name="toolAnimator">Reference to the tool animator</param>
        /// <param name="rigidbody2D">Reference to the player's rigidbody</param>
        public PlayerTakingDamageState(PlayerContoller playerContoller, Animator animator, Animator toolAnimator, Rigidbody2D rigidbody2D) : base(playerContoller, animator, toolAnimator, rigidbody2D)
        {
        }
        
        /// <summary>
        /// Sets up animation parameters and calculates knockback direction
        /// </summary>
        public override void OnEnter()
        {
            base.OnEnter();
            
            SetupHitDirectionForAnimation();
            
            Animator.CrossFade(TakingDamageAnimHash, CrossFadeTime);
            
            CalculateKnockbackPosition();
        }

        /// <summary>
        /// Sets the hit direction parameters for the animation
        /// </summary>
        private void SetupHitDirectionForAnimation()
        {
            Vector2 selfPosition = PlayerContoller.transform.position;
            Animator.SetFloat(HitX, HitInfo.HitPoint.x - selfPosition.x);
            Animator.SetFloat(HitY, HitInfo.HitPoint.y - selfPosition.y);
        }

        /// <summary>
        /// Calculates the position the player should be knocked back to
        /// </summary>
        private void CalculateKnockbackPosition()
        {
            Vector3 knockBackDirection = ((Vector2)PlayerContoller.transform.position - HitInfo.HitPoint).normalized;
            knockBackPosition = PlayerContoller.transform.position + KnockBackDistance * knockBackDirection;
        }

        /// <summary>
        /// Handles the knock back movement
        /// </summary>
        public override void OnUpdate()
        {
            base.OnUpdate();
            
            PlayerContoller.transform.position = Vector3.MoveTowards(
                PlayerContoller.transform.position,
                knockBackPosition,
                KnockBackSpeed * Time.deltaTime);
        }

        /// <summary>
        /// When the "EndTakingDamage" event is received, the player is no longer taking damage
        /// </summary>
        /// <param name="animationEvent">Data associated with the animation event</param>
        public override void OnAnimationEvent(AnimationEvent animationEvent)
        {
            base.OnAnimationEvent(animationEvent);
            
            if (animationEvent.stringParameter == "EndTakingDamage")
            {
                PlayerContoller.IsTakingDamage = false;
            }
        }
    }
}