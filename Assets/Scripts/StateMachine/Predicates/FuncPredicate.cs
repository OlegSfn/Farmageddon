using System;

namespace Enemies.FSM.StateMachine.Predicates
{
    public class FuncPredicate : IPredicate
    {
        private readonly Func<bool> _func;
        
        public FuncPredicate(Func<bool> func) {
            _func = func;
        }
        
        public bool IsTrue() => _func.Invoke();
    }
}
