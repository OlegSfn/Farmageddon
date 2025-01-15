using Enemies.FSM.StateMachine.Predicates;

namespace Enemies.FSM.StateMachine.Transitions
{
    public class Transition : ITransition
    {
        public StateNode TargetState { get; }
        public IPredicate Condition { get; }
    
        public Transition(StateNode targetState, IPredicate condition)
        {
            TargetState = targetState;
            Condition = condition;
        }
    }
}
