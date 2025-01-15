using System.Collections.Generic;
using Enemies.FSM.StateMachine.Predicates;
using Enemies.FSM.StateMachine.Transitions;

namespace Enemies.FSM.StateMachine
{
    public class StateNode
    {
        public IState State { get; }
        public HashSet<Transition> Transitions { get; }
    
        public StateNode(IState state)
        {
            State = state;
            Transitions = new HashSet<Transition>();
        }
    
        public void AddTransition(StateNode targetState, IPredicate condition)
        {
            Transitions.Add(new Transition(targetState, condition));
        }
    }
}
