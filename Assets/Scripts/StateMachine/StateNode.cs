using System.Collections.Generic;
using Enemies.FSM.StateMachine.Predicates;
using Enemies.FSM.StateMachine.Transitions;

namespace Enemies.FSM.StateMachine
{
    /// <summary>
    /// Represents a node in the state machine graph
    /// Encapsulates a state and its outgoing transitions
    /// </summary>
    public class StateNode
    {
        /// <summary>
        /// The state implementation contained in this node
        /// </summary>
        public IState State { get; }
        
        /// <summary>
        /// Collection of all transitions that can occur from this state
        /// </summary>
        public HashSet<Transition> Transitions { get; }
    
        /// <summary>
        /// Creates a new state node with the specified state implementation
        /// </summary>
        /// <param name="state">The state to encapsulate in this node</param>
        public StateNode(IState state)
        {
            State = state;
            Transitions = new HashSet<Transition>();
        }
    
        /// <summary>
        /// Adds a new transition from this state to a target state
        /// </summary>
        /// <param name="targetState">The state node to transition to</param>
        /// <param name="condition">The condition that must be true for this transition to occur</param>
        public void AddTransition(StateNode targetState, IPredicate condition)
        {
            Transitions.Add(new Transition(targetState, condition));
        }
    }
}