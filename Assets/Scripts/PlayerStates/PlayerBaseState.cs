using Enemies.FSM.StateMachine;
using Managers;
using UnityEngine;

namespace PlayerStates
{
    /// <summary>
    /// Abstract base class for all player states in the state machine
    /// Provides common functionality and references needed by all states
    /// </summary>
    public abstract class PlayerBaseState : IState
    {
        /// <summary>
        /// Reference to the main player controller
        /// </summary>
        protected readonly PlayerContoller PlayerContoller;
        
        /// <summary>
        /// Reference to the player's physics component
        /// </summary>
        protected readonly Rigidbody2D Rigidbody2D;
        
        /// <summary>
        /// Reference to the player's main animator
        /// </summary>
        protected readonly Animator Animator;
        
        /// <summary>
        /// Reference to the tool animator (separate from player's main animator)
        /// </summary>
        protected readonly Animator ToolAnimator;
        
        /// <summary>
        /// Time for crossfading between animations
        /// </summary>
        protected const float CrossFadeTime = 0f;
        
        /// <summary>
        /// Animation parameter hashes for better performance
        /// </summary>
        protected static readonly int WateringAnimHash = Animator.StringToHash("Watering");
        protected static readonly int AttackingAnimHash = Animator.StringToHash("Attacking");
        protected static readonly int WeedingAnimHash = Animator.StringToHash("Weeding");
        protected static readonly int IdleAnimHash = Animator.StringToHash("Idle");
        protected static readonly int MovementAnimHash = Animator.StringToHash("Movement");
        protected static readonly int TakingDamageAnimHash = Animator.StringToHash("TakingDamage");
        protected static readonly int DyingAnimHash = Animator.StringToHash("Dying");
        protected static readonly int UseToolAnimHash = Animator.StringToHash("Use");
        protected static readonly int IdleToolAnimHash = Animator.StringToHash("IdleTool");
        
        /// <summary>
        /// Animation parameter hashes for direction and velocity for better performance
        /// </summary>
        private static readonly int LookDirXHash = Animator.StringToHash("LookDirX");
        private static readonly int LookDirYHash = Animator.StringToHash("LookDirY");
        private static readonly int VelX = Animator.StringToHash("VelX");
        private static readonly int VelY = Animator.StringToHash("VelY");

        /// <summary>
        /// Name of this state for state machine to check on equality
        /// </summary>
        public abstract string Name { get; }
        
        /// <summary>
        /// Constructor for the base state, initializing required components
        /// </summary>
        /// <param name="playerContoller">Reference to the player controller</param>
        /// <param name="animator">Reference to the player's main animator</param>
        /// <param name="toolAnimator">Reference to the tool animator</param>
        /// <param name="rigidbody2D">Reference to the player's rigidbody</param>
        protected PlayerBaseState(PlayerContoller playerContoller, Animator animator, Animator toolAnimator, Rigidbody2D rigidbody2D)
        {
            PlayerContoller = playerContoller;
            Animator = animator;
            ToolAnimator = toolAnimator;
            Rigidbody2D = rigidbody2D;
        }

        /// <summary>
        /// Called when the state is entered
        /// Override in derived classes to perform state-specific initialization
        /// </summary>
        public virtual void OnEnter() { }

        /// <summary>
        /// Called when the state is exited
        /// Override in derived classes to perform state-specific cleanup
        /// </summary>
        public virtual void OnExit() { }

        /// <summary>
        /// Called every frame while this state is active
        /// Updates animation parameters based on player input and look direction
        /// </summary>
        public virtual void OnUpdate()
        {
            if (PlayerContoller.IsTakingDamage || !PlayerContoller.IsAlive || GameManager.Instance.IsPaused)
            {
                return;
            }
            
            UpdateAnimationParameters();
        }

        /// <summary>
        /// Called every frame while this state is active
        /// Updates all animation parameters for both player and tool animators
        /// </summary>
        private void UpdateAnimationParameters()
        {
            Animator.SetFloat(VelX, PlayerContoller.Input.x);
            Animator.SetFloat(VelY, PlayerContoller.Input.y);
            Animator.SetFloat(LookDirXHash, PlayerContoller.LookDirection.x);
            Animator.SetFloat(LookDirYHash, PlayerContoller.LookDirection.y);
            
            ToolAnimator.SetFloat(LookDirXHash, PlayerContoller.LookDirection.x);
            ToolAnimator.SetFloat(LookDirYHash, PlayerContoller.LookDirection.y);
        }

        /// <summary>
        /// Called at fixed intervals for physics calculations while this state is active
        /// Override in derived classes to implement physics-based behavior
        /// </summary>
        public virtual void OnFixedUpdate() { }

        /// <summary>
        /// Handles animation events triggered during this state
        /// Override in derived classes to handle animation events
        /// </summary>
        /// <param name="animationEvent">Data associated with the animation event</param>
        public virtual void OnAnimationEvent(AnimationEvent animationEvent) { }
    }
}