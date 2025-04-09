using StateMachine;
using Managers;
using UnityEngine;
using UnityEngine.AI;

namespace Enemies.Slime.SlimeStates
{
    /// <summary>
    /// Base abstract class for all slime states providing common functionality
    /// and references needed by derived state classes
    /// </summary>
    public abstract class SlimeBaseState : IState
    {
        /// <summary>
        /// Reference to the parent slime this state belongs to
        /// </summary>
        protected readonly Slime Slime;
        
        /// <summary>
        /// Reference to the navigation agent for movement control
        /// </summary>
        protected readonly NavMeshAgent NavMeshAgent;
        
        /// <summary>
        /// Reference to the animator component for animation control
        /// </summary>
        protected readonly Animator Animator;
        
        /// <summary>
        /// Time in seconds for animation crossfading between states
        /// </summary>
        protected const float CrossFadeTime = 0.1f;

        /// <summary>
        /// Animation parameter hashes for better performance
        /// </summary>
        protected static readonly int WalkingAnimHash = Animator.StringToHash("Walking");
        protected static readonly int ChasingAnimHash = Animator.StringToHash("Chasing");
        protected static readonly int DyingAnimHash = Animator.StringToHash("Dying");
        protected static readonly int TakingDamageAnimHash = Animator.StringToHash("TakingDamage");
        
        /// <summary>
        /// Animation velocity parameter hashes for better performance
        /// </summary>
        private static readonly int VelX = Animator.StringToHash("VelX");
        private static readonly int VelY = Animator.StringToHash("VelY");
        
        /// <summary>
        /// Name of this state for state machine to check on equality
        /// </summary>
        public abstract string Name { get; }
    
        /// <summary>
        /// Constructor that initializes the base state with required references
        /// </summary>
        /// <param name="slime">The slime this state belongs to</param>
        /// <param name="navMeshAgent">The navigation agent for movement</param>
        /// <param name="animator">The animator component for animations</param>
        protected SlimeBaseState(Slime slime, NavMeshAgent navMeshAgent, Animator animator)
        {
            Slime = slime;
            NavMeshAgent = navMeshAgent;
            Animator = animator;
        }
    
        /// <summary>
        /// Called when the state is entered
        /// Override in derived classes to provide state-specific initialization
        /// </summary>
        public virtual void OnEnter() {}

        /// <summary>
        /// Called when the state is exited
        /// Override in derived classes to provide state-specific cleanup
        /// </summary>
        public virtual void OnExit() {}

        /// <summary>
        /// Called every frame while this state is active
        /// Updates animator parameters with current velocity
        /// </summary>
        public virtual void OnUpdate()
        {
            Vector2 velocity = NavMeshAgent.velocity.normalized;
            Animator.SetFloat(VelX, velocity.x);
            Animator.SetFloat(VelY, velocity.y);
        }

        /// <summary>
        /// Called at fixed intervals for physics calculations while this state is active
        /// Override in derived classes to provide state-specific physics behavior
        /// </summary>
        public virtual void OnFixedUpdate() {}

        /// <summary>
        /// Handles animation events triggered during this state
        /// Override in derived classes to handle animation events
        /// When the "Jumped" event is received, a sound is played
        /// </summary>
        /// <param name="animationEvent">Data from the animation event</param>
        public virtual void OnAnimationEvent(AnimationEvent animationEvent)
        {
            if (animationEvent.stringParameter == "Jumped")
            {
                AudioManager.Instance.PlaySlimeJumpSound(Slime.transform.position);
            }
        }
    }
}