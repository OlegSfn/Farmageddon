using System.Collections.Generic;
using Enemies.FSM.StateMachine.Predicates;
using Enemies.FSM.StateMachine.Transitions;
using UnityEngine;

namespace Enemies.FSM.StateMachine
{
    public class StateMachine
    {
        private StateNode _currentState;
        private readonly Dictionary<string, StateNode> _states = new();
        private readonly HashSet<ITransition> _anyStateTransitions = new();
    
        public void Update()
        {
            ITransition transition = GetValidTransition();
            if (transition is not null)
            {
                DoTransition(transition);
            }
        
            _currentState.State?.OnUpdate();
        }
    
        public void FixedUpdate()
        {
            _currentState.State?.OnFixedUpdate();
        }
    
        public void AnimationEvent(AnimationEvent animationEvent)
        {
            _currentState.State?.OnAnimationEvent(animationEvent);
        }
    
        public void StartEntryState(IState state)
        {
            _currentState = _states[state.Name];
            _currentState.State?.OnEnter();
        }

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
    
        private void DoTransition(ITransition transition)
        {
            SetState(transition.TargetState.State);
        }
    
        ITransition GetValidTransition()
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
    
        public void AddTransition(IState sourceState, IState targetState, IPredicate condition)
        {
            var sourceNode = GetOrAddState(sourceState);
            var targetNode = GetOrAddState(targetState);
        
            sourceNode.AddTransition(targetNode, condition);
        }

        public void AddAnyTransition(IState targetState, IPredicate condition)
        {
            var targetNode = GetOrAddState(targetState);
            _anyStateTransitions.Add(new Transition(targetNode, condition));
        }
    }
}
