using Enemies.FSM.StateMachine.Predicates;

namespace Enemies.FSM.StateMachine.Transitions
{
    /// <summary>
    /// Interface for transitions between states in the state machine
    /// Defines the core properties that all transitions must implement
    /// </summary>
    public interface ITransition
    {
        /// <summary>
        /// The destination state node that this transition leads to
        /// </summary>
        StateNode TargetState { get; }
        
        /// <summary>
        /// The condition that must be met for this transition to be taken
        /// </summary>
        IPredicate Condition { get; }
    }
}