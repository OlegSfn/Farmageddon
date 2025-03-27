using System.Collections.Generic;
using Enemies.FSM.StateMachine.Predicates;
using Enemies.FSM.StateMachine.Transitions;
using UnityEngine;

namespace Enemies.FSM.StateMachine
{
    /// <summary>
    /// A flexible Finite State Machine implementation for game entity behavior control
    /// Manages states, transitions between states, and conditional logic
    /// </summary>
    public class StateMachine
    {
        /// <summary>
        /// The current active state node in the state machine
        /// </summary>
        private StateNode _currentState;
        
        /// <summary>
        /// Dictionary mapping state names to their corresponding state nodes
        /// </summary>
        private readonly Dictionary<string, StateNode> _states = new();
        
        /// <summary>
        /// Collection of transitions that can occur from any state
        /// </summary>
        private readonly HashSet<ITransition> _anyStateTransitions = new();
    
        /// <summary>
        /// Updates the state machine each frame
        /// Checks for valid transitions and executes the current state's update logic
        /// </summary>
        public void Update()
        {
            ITransition transition = GetValidTransition();
            if (transition is not null)
            {
                DoTransition(transition);
            }
        
            _currentState.State?.OnUpdate();
        }
    
        /// <summary>
        /// Executes the current state's fixed update logic
        /// Called during the physics update cycle
        /// </summary>
        public void FixedUpdate()
        {
            _currentState.State?.OnFixedUpdate();
        }
    
        /// <summary>
        /// Forwards animation events to the current state
        /// </summary>
        /// <param name="animationEvent">The animation event data to forward</param>
        public void AnimationEvent(AnimationEvent animationEvent)
        {
            _currentState.State?.OnAnimationEvent(animationEvent);
        }
    
        /// <summary>
        /// Sets the initial state of the state machine and enters it
        /// </summary>
        /// <param name="state">The state to use as the entry point</param>
        public void StartEntryState(IState state)
        {
            _currentState = _states[state.Name];
            _currentState.State?.OnEnter();
        }

        /// <summary>
        /// Changes the current state to a specified target state
        /// Handles exit and entry logic for the states
        /// </summary>
        /// <param name="targetState">The state to transition to</param>
        public void SetState(IState targetState)
        {
            if (_currentState == _states[targetState.Name])
            {
                return;
            }
        
            _currentState.State?.OnExit();
            
            _currentState = _states[targetState.Name];
            _currentState.State?.OnEnter();
        }
    
        /// <summary>
        /// Performs a transition to the target state specified in the transition
        /// </summary>
        /// <param name="transition">The transition to execute</param>
        private void DoTransition(ITransition transition)
        {
            SetState(transition.TargetState.State);
        }
    
        /// <summary>
        /// Finds the first valid transition that can be taken from the current state
        /// Checks any-state transitions first, then state-specific transitions
        /// </summary>
        /// <returns>The first valid transition, or null if none are valid</returns>
        private ITransition GetValidTransition()
        {
            foreach (var transition in _anyStateTransitions)
            {
                if (transition.Condition.IsTrue())
                {
                    return transition;
                }
            }
        
            foreach (var transition in _currentState.Transitions)
            {
                if (transition.Condition.IsTrue())
                {
                    return transition;
                }
            }
        
            return null;
        }

        /// <summary>
        /// Retrieves an existing state node or creates a new one if it doesn't exist
        /// </summary>
        /// <param name="state">The state to get or create a node for</param>
        /// <returns>The existing or newly created state node</returns>
        private StateNode GetOrAddState(IState state)
        {
            if (_states.TryGetValue(state.Name, out var existedState))
            {
                return existedState;
            }
        
            var stateNode = new StateNode(state);
            _states.Add(state.Name, stateNode);
            return stateNode;
        }
    
        /// <summary>
        /// Adds a conditional transition between two states
        /// </summary>
        /// <param name="sourceState">The state where the transition begins</param>
        /// <param name="targetState">The state where the transition ends</param>
        /// <param name="condition">The condition that must be true for the transition to occur</param>
        public void AddTransition(IState sourceState, IState targetState, IPredicate condition)
        {
            var sourceNode = GetOrAddState(sourceState);
            var targetNode = GetOrAddState(targetState);
        
            sourceNode.AddTransition(targetNode, condition);
        }

        /// <summary>
        /// Adds a transition that can occur from any state when its condition is met
        /// </summary>
        /// <param name="targetState">The state to transition to</param>
        /// <param name="condition">The condition that must be true for the transition to occur</param>
        public void AddAnyTransition(IState targetState, IPredicate condition)
        {
            var targetNode = GetOrAddState(targetState);
            _anyStateTransitions.Add(new Transition(targetNode, condition));
        }
    }
}
