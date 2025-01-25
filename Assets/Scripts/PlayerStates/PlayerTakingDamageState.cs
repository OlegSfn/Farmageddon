using UnityEngine;

namespace PlayerStates
{
    public class PlayerTakingDamageState : PlayerBaseState
    {
        public override string Name => "TakingDamage";
        
        private Vector2 knockBackPosition;
        
        private const float KnockBackDistance = 1f;
        private const float KnockBackSpeed = 7f;
        private static readonly int HitX = Animator.StringToHash("HitX");
        private static readonly int HitY = Animator.StringToHash("HitY");

        public HitInfo HitInfo { get; set; }
        
        public PlayerTakingDamageState(PlayerContoller playerContoller, Animator animator, Rigidbody2D rigidbody2D) : base(playerContoller, animator, rigidbody2D)
        {
        }
        
        public override void OnEnter()
        {
            base.OnEnter();
            Vector2 selfPosition = PlayerContoller.transform.position;
            Animator.SetFloat(HitX, HitInfo.HitPoint.x - selfPosition.x);
            Animator.SetFloat(HitY, HitInfo.HitPoint.y - selfPosition.y);
            Animator.CrossFade(TakingDamageAnimHash, CrossFadeTime);
            
            Vector3 knockBackDirection = ((Vector2)PlayerContoller.transform.position - HitInfo.HitPoint).normalized;
            knockBackPosition = PlayerContoller.transform.position + KnockBackDistance*knockBackDirection;
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
            PlayerContoller.transform.position = Vector3.MoveTowards(PlayerContoller.transform.position, knockBackPosition, KnockBackSpeed * Time.deltaTime);
        }


        public override void OnAnimationEvent(AnimationEvent animationEvent)
        {
            base.OnAnimationEvent(animationEvent);
            if (animationEvent.stringParameter == "EndTakingDamage")
            {
                PlayerContoller.IsTakingDamage = false;
            }
        }
    }
}