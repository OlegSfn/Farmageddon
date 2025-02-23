using Enemies.FSM.StateMachine;
using Enemies.FSM.StateMachine.Predicates;
using Inventory;
using Managers;
using PlayerStates;
using UnityEngine;

[RequireComponent(typeof(Animator)), RequireComponent(typeof(Rigidbody2D))]
public class PlayerContoller : MonoBehaviour
{
    [field: SerializeField] public float Speed { get; private set; }

    [field: SerializeField] public Animator ToolAnimator { get; set; }
    [field: SerializeField] private Collider2D toolCollder;

    public Vector2 Input { get; private set; }
    public Vector2 LookDirection { get; private set; }
    
    public bool IsWeeding { get; set; }
    public bool IsAttacking { get; set; }
    public bool IsWatering { get; set; }
    public bool IsAlive { get; set; }
    public bool IsTakingDamage { get; set; }
    
    private Animator _animator;
    private Rigidbody2D _rb;
    private Camera _mainCamera;
    private StateMachine _stateMachine;
    private PlayerTakingDamageState _takingDamageState;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _rb = GetComponent<Rigidbody2D>();
        _mainCamera = Camera.main;
        IsAlive = true;
    }

    private void Start()
    {
        InitStateMachine();
    }

    void InitStateMachine()
    {
        _stateMachine = new StateMachine();

        var attackingState = new PlayerAttackingState(this, _animator, ToolAnimator, _rb, toolCollder);
        var movementState = new PlayerMovementState(this, _animator, ToolAnimator, _rb);
        var wateringState = new PlayerWateringState(this, _animator, ToolAnimator, _rb);
        var weedingState = new PlayerWeedingState(this, _animator, ToolAnimator, _rb);
        var dyingState = new PlayerDyingState(this, _animator, ToolAnimator, _rb);
        _takingDamageState = new PlayerTakingDamageState(this, _animator, ToolAnimator, _rb);


        bool IsAbleToAct() => IsAlive && !IsTakingDamage;
        _stateMachine.AddAnyTransition(attackingState, new FuncPredicate(() => IsAbleToAct() && IsAttacking));
        _stateMachine.AddAnyTransition(wateringState, new FuncPredicate(() => IsAbleToAct() && IsWatering));
        _stateMachine.AddAnyTransition(weedingState, new FuncPredicate(() => IsAbleToAct() && IsWeeding));
        _stateMachine.AddAnyTransition(_takingDamageState, new FuncPredicate(() => IsTakingDamage && IsAlive));
        _stateMachine.AddAnyTransition(dyingState, new FuncPredicate(() => !IsAlive));
        
        _stateMachine.AddTransition(attackingState, movementState, new FuncPredicate(() => !IsAttacking));
        _stateMachine.AddTransition(wateringState, movementState, new FuncPredicate(() => !IsWatering));
        _stateMachine.AddTransition(weedingState, movementState, new FuncPredicate(() => !IsWeeding));
        _stateMachine.AddTransition(_takingDamageState, movementState, new FuncPredicate(() => !IsTakingDamage));
        
        _stateMachine.StartEntryState(movementState);
    }

    private void FixedUpdate()
    {
        _stateMachine.FixedUpdate();
    }

    void Update()
    {
        ReadInput();
        _stateMachine.Update();
    }
    
    public void AnimationEvent(AnimationEvent animationEvent)
    {
        _stateMachine.AnimationEvent(animationEvent);
    }

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
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        InventoryItem item = other.GetComponent<InventoryItem>();
        if (item != null && item.IsPickable)
        {
            GameManager.Instance.inventory.AddItems(item, item.Quantity);
        }
    }
    
    public void TakeDamage(HitInfo hitInfo, int _)
    {
        _takingDamageState.HitInfo = hitInfo;
        IsTakingDamage = true;
    }

    public void Die()
    {
        IsAlive = false;
    }
}
