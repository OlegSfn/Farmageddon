using Helpers;
using Inventory;
using Managers;
using Player.PlayerStates;
using StateMachine.Predicates;
using UnityEngine;

namespace Player
{
    /// <summary>
    /// Main controller for player character handling movement, combat, and state management
    /// </summary>
    [RequireComponent(typeof(Animator)), RequireComponent(typeof(Rigidbody2D))]
    public class PlayerController : MonoBehaviour
    {
        /// <summary>
        /// Base movement speed of the player
        /// </summary>
        [field: SerializeField] public float Speed { get; private set; }

        /// <summary>
        /// Animator controller for tool animations
        /// </summary>
        [field: SerializeField] public Animator ToolAnimator { get; set; }
    
        /// <summary>
        /// Collider for tool interaction detection
        /// </summary>
        [field: SerializeField] private Collider2D toolCollder;

        /// <summary>
        /// Current movement input vector
        /// </summary>
        public Vector2 Input { get; private set; }
    
        /// <summary>
        /// Normalized direction the player is facing
        /// </summary>
        public Vector2 LookDirection { get; private set; }
    
        // State flags.
        public bool IsWeeding { get; set; }
        public bool IsAttacking { get; set; }
        public bool IsWatering { get; set; }
        public bool IsAlive { get; private set; }
        public bool IsTakingDamage { get; set; }
        
        public bool IsAbleToUseItem => !(IsWatering || IsWeeding || IsAttacking) && IsAlive && !IsTakingDamage;
        
        /// <summary>
        /// Controls whether player can receive movement input
        /// </summary>
        [field: SerializeField] public bool CanMove { get; set; }
    
        // Component references.
        private Animator _animator;
        private Rigidbody2D _rb;
        private Camera _mainCamera;
        private StateMachine.StateMachine _stateMachine;
        private PlayerTakingDamageState _takingDamageState;

        /// <summary>
        /// Initialize component references and default states
        /// </summary>
        private void Awake()
        {
            _animator = GetComponent<Animator>();
            _rb = GetComponent<Rigidbody2D>();
            _mainCamera = Camera.main;
            IsAlive = true;
            CanMove = true;
        }

        private void Start()
        {
            InitStateMachine();
        }

        /// <summary>
        /// Configures the player's state machine with all possible states and transitions
        /// </summary>
        private void InitStateMachine()
        {
            _stateMachine = new StateMachine.StateMachine();

            var attackingState = new PlayerAttackingState(this, _animator, ToolAnimator, _rb, toolCollder);
            var movementState = new PlayerMovementState(this, _animator, ToolAnimator, _rb);
            var wateringState = new PlayerWateringState(this, _animator, ToolAnimator, _rb);
            var weedingState = new PlayerWeedingState(this, _animator, ToolAnimator, _rb);
            var dyingState = new PlayerDyingState(this, _animator, ToolAnimator, _rb);
            _takingDamageState = new PlayerTakingDamageState(this, _animator, ToolAnimator, _rb);

            bool IsAbleToAct() => IsAlive && !IsTakingDamage && !GameManager.Instance.IsPaused;
        
            _stateMachine.AddTransition(movementState, attackingState, new FuncPredicate(() => IsAbleToAct() && IsAttacking));
            _stateMachine.AddTransition(movementState, wateringState, new FuncPredicate(() => IsAbleToAct() && IsWatering));
            _stateMachine.AddTransition(movementState, weedingState, new FuncPredicate(() => IsAbleToAct() && IsWeeding));
            _stateMachine.AddAnyTransition(_takingDamageState, new FuncPredicate(() => IsTakingDamage && IsAlive));
            _stateMachine.AddAnyTransition(dyingState, new FuncPredicate(() => !IsAlive));
        
            _stateMachine.AddTransition(attackingState, movementState, new FuncPredicate(() => !IsAttacking));
            _stateMachine.AddTransition(wateringState, movementState, new FuncPredicate(() => !IsWatering));
            _stateMachine.AddTransition(weedingState, movementState, new FuncPredicate(() => !IsWeeding));
            _stateMachine.AddTransition(_takingDamageState, movementState, new FuncPredicate(() => !IsTakingDamage));
        
            _stateMachine.StartEntryState(movementState);
        }

        /// <summary>
        /// Handles physics updates for the state machine
        /// </summary>
        private void FixedUpdate()
        {
            _stateMachine.FixedUpdate();
        }

        /// <summary>
        /// Main update loop for input processing and state updates
        /// </summary>
        private void Update()
        {
            ReadInput();
            _stateMachine.Update();
        }
    
        /// <summary>
        /// Forwards animation events to the current state
        /// </summary>
        /// <param name="animationEvent">Event data from animation timeline</param>
        public void AnimationEvent(AnimationEvent animationEvent)
        {
            _stateMachine.AnimationEvent(animationEvent);
        }

        /// <summary>
        /// Reads and processes player input for movement and look direction
        /// </summary>
        private void ReadInput()
        {
            Input = new Vector2(UnityEngine.Input.GetAxisRaw("Horizontal"), UnityEngine.Input.GetAxisRaw("Vertical"));
            Input = Speed * Input.normalized;
        
            Vector3 mousePosition = UnityEngine.Input.mousePosition;
            mousePosition = _mainCamera.ScreenToWorldPoint(mousePosition);
            var playerPosition = transform.position;
            Vector2 direction = new Vector2(mousePosition.x - playerPosition.x, mousePosition.y - playerPosition.y);
            direction.Normalize();
            LookDirection = new Vector2(
                Mathf.Round(direction.x),
                Mathf.Round(direction.y)
            );
        }
    
        /// <summary>
        /// Handles inventory item pickup collisions
        /// </summary>
        /// <param name="other">The collider entered</param>
        private void OnTriggerEnter2D(Collider2D other)
        {
            InventoryItem item = other.GetComponent<InventoryItem>();
            if (item != null && item.IsPickable)
            {
                GameManager.Instance.inventory.AddItems(item, item.Quantity);
            }
        }
    
        /// <summary>
        /// Applies damage to the player and triggers damage state
        /// </summary>
        /// <param name="hitInfo">Information about the damage source</param>
        /// <param name="_">Unused parameter (required by interface)</param>
        public void TakeDamage(HitInfo? hitInfo, int _)
        {
            if (!hitInfo.HasValue)
            {
                return;
            }
        
            _takingDamageState.HitInfo = hitInfo.Value;
            IsTakingDamage = true;
            AudioManager.Instance.PlayPlayerHurtSound(transform.position);
        }

        /// <summary>
        /// Handles player death sequence and game over
        /// </summary>
        public void Die()
        {
            AudioManager.Instance.PlayPlayerHurtSound(transform.position);
            IsAlive = false;
            GameManager.Instance.GameOver();
        }
    }
}
