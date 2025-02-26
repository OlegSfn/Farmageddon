using Enemies.FSM.StateMachine;
using UnityEngine;
using UnityEngine.AI;

namespace Enemies.Slime.SlimeStates
{
    public abstract class SlimeBaseState : IState
    {
        protected readonly Slime Slime;
        protected readonly NavMeshAgent NavMeshAgent;
        protected readonly Animator Animator;
        protected const float CrossFadeTime = 0.1f;

        protected static readonly int WalkingAnimHash = Animator.StringToHash("Walking");
        protected static readonly int ChasingAnimHash = Animator.StringToHash("Chasing");
        protected static readonly int DyingAnimHash = Animator.StringToHash("Dying");
        protected static readonly int TakingDamageAnimHash = Animator.StringToHash("TakingDamage");
        
        private static readonly int VelX = Animator.StringToHash("VelX");
        private static readonly int VelY = Animator.StringToHash("VelY");
        public abstract string Name { get; }
    
        protected SlimeBaseState(Slime slime, NavMeshAgent navMeshAgent, Animator animator)
        {
            Slime = slime;
            NavMeshAgent = navMeshAgent;
            Animator = animator;
        }
    
        public virtual void OnEnter() {}

        public virtual void OnExit() {}

        public virtual void OnUpdate()
        {
            Vector2 velocity = NavMeshAgent.velocity.normalized;
            Animator.SetFloat(VelX, velocity.x);
            Animator.SetFloat(VelY, velocity.y);
        }

        public virtual void OnFixedUpdate() {}
    
        public virtual void OnAnimationEvent(AnimationEvent animationEvent) {}
    }
}