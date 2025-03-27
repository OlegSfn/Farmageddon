namespace Enemies.FSM.StateMachine.Predicates
{
    /// <summary>
    /// Interface for conditional predicates used in state machine transitions
    /// Represents a boolean condition that can be evaluated to determine if a transition should occur
    /// </summary>
    public interface IPredicate
    {
        /// <summary>
        /// Evaluates whether the condition represented by this predicate is currently true
        /// </summary>
        /// <returns>True if the condition is met, false otherwise</returns>
        bool IsTrue();
    }
}