using Enemies.FSM.StateMachine;
using UnityEngine;

namespace PlayerStates
{
    public abstract class PlayerBaseState : IState
    {
        protected readonly PlayerContoller PlayerContoller;
        protected readonly Rigidbody2D Rigidbody2D;
        protected readonly Animator Animator;
        protected const float CrossFadeTime = 0f;
        
        protected static readonly int WateringAnimHash = Animator.StringToHash("Watering");
        protected static readonly int AttackingAnimHash = Animator.StringToHash("Attacking");
        protected static readonly int WeedingAnimHash = Animator.StringToHash("Weeding");
        protected static readonly int IdleAnimHash = Animator.StringToHash("Idle");
        protected static readonly int MovementAnimHash = Animator.StringToHash("Movement");
        
        private static readonly int LookDirXHash = Animator.StringToHash("LookDirX");
        private static readonly int LookDirYHash = Animator.StringToHash("LookDirY");
        private static readonly int VelX = Animator.StringToHash("VelX");
        private static readonly int VelY = Animator.StringToHash("VelY");

        public abstract string Name { get; }
        
        protected PlayerBaseState(PlayerContoller playerContoller, Animator animator, Rigidbody2D rigidbody2D)
        {
            PlayerContoller = playerContoller;
            Animator = animator;
            Rigidbody2D = rigidbody2D;
        }

        public virtual void OnEnter() { }

        public virtual void OnExit() { }

        public virtual void OnUpdate()
        {
            Animator.SetFloat(VelX, PlayerContoller.Input.x);
            Animator.SetFloat(VelY, PlayerContoller.Input.y);
            Animator.SetFloat(LookDirXHash, PlayerContoller.LookDirection.x);
            Animator.SetFloat(LookDirYHash, PlayerContoller.LookDirection.y);
        }

        public virtual void OnFixedUpdate() { }

        public virtual void OnAnimationEvent(AnimationEvent animationEvent) { }
    }
}