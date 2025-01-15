using UnityEngine;

namespace Enemies.FSM.StateMachine
{
    public interface IState
    {
        public string Name { get; }
    
        public void OnEnter();
        public void OnExit();
    
        public void OnUpdate();
        public void OnFixedUpdate();
    
        public void OnAnimationEvent(AnimationEvent animationEvent);
    }
}
