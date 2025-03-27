using System;

namespace Enemies.FSM.StateMachine.Predicates
{
    /// <summary>
    /// A flexible implementation of IPredicate that uses a function delegate to determine the result
    /// Allows for creating predicates using lambda expressions or method references
    /// </summary>
    public class FuncPredicate : IPredicate
    {
        /// <summary>
        /// The function that will be evaluated to determine if the predicate is true
        /// </summary>
        private readonly Func<bool> _func;
        
        /// <summary>
        /// Creates a new predicate based on the provided boolean function
        /// </summary>
        /// <param name="func">A function that returns true when the condition is met</param>
        public FuncPredicate(Func<bool> func) {
            _func = func;
        }
        
        /// <summary>
        /// Evaluates the predicate by invoking the stored function
        /// </summary>
        /// <returns>The result of the function evaluation</returns>
        public bool IsTrue() => _func.Invoke();
    }
}