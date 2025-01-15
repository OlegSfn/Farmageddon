using Enemies.FSM.StateMachine.Predicates;

namespace Enemies.FSM.StateMachine.Transitions
{
    public interface ITransition
    {
        StateNode TargetState { get; }
        IPredicate Condition { get; }
    }
}
