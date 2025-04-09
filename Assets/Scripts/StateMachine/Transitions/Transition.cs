using StateMachine.Predicates;

namespace StateMachine.Transitions
{
    /// <summary>
    /// Implementation of a transition between states in the state machine
    /// Connects a source state to a target state with a conditional predicate
    /// </summary>
    public class Transition : ITransition
    {
        /// <summary>
        /// The state node that this transition leads to when activated
        /// </summary>
        public StateNode TargetState { get; }
        
        /// <summary>
        /// The condition that must be satisfied for this transition to occur
        /// </summary>
        public IPredicate Condition { get; }
    
        /// <summary>
        /// Creates a new transition to the specified target state with the given condition
        /// </summary>
        /// <param name="targetState">The state to transition to when the condition is met</param>
        /// <param name="condition">The condition that triggers this transition</param>
        public Transition(StateNode targetState, IPredicate condition)
        {
            TargetState = targetState;
            Condition = condition;
        }
    }
}