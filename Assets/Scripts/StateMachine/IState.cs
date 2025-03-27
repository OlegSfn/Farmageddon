using UnityEngine;

namespace Enemies.FSM.StateMachine
{
    /// <summary>
    /// Interface for state objects in the Finite State Machine
    /// Defines the core methods that all states must implement
    /// </summary>
    public interface IState
    {
        /// <summary>
        /// Unique identifier for this state
        /// </summary>
        public string Name { get; }
    
        /// <summary>
        /// Called when the state machine enters this state
        /// Use for initialization and setup specific to this state
        /// </summary>
        public void OnEnter();
        
        /// <summary>
        /// Called when the state machine exits this state
        /// Use for cleanup and finalization before transitioning to another state
        /// </summary>
        public void OnExit();
    
        /// <summary>
        /// Called every frame while this state is active
        /// Use for regular non-physics updates
        /// </summary>
        public void OnUpdate();
        
        /// <summary>
        /// Called at fixed time intervals while this state is active
        /// Use for physics-related updates
        /// </summary>
        public void OnFixedUpdate();
    
        /// <summary>
        /// Called when an animation event is triggered while this state is active
        /// Use to synchronize code execution with animation keyframes
        /// </summary>
        /// <param name="animationEvent">Data from the animation event</param>
        public void OnAnimationEvent(AnimationEvent animationEvent);
    }
}