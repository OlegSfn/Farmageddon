using System;
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
    [field: SerializeField] private Collider2D toolCollder;

    public Vector2 Input { get; private set; }
    public Vector2 LookDirection { get; private set; }
    
    public bool IsWeeding { get; set; }
    public bool IsAttacking { get; set; }
    public bool IsWatering { get; set; }
    
    private Animator _animator;
    private Rigidbody2D _rb;
    private Camera _mainCamera;
    private StateMachine _stateMachine;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _rb = GetComponent<Rigidbody2D>();
        _mainCamera = Camera.main;
    }

    private void Start()
    {
        InitStateMachine();
    }

    void InitStateMachine()
    {
        _stateMachine = new StateMachine();

        var attackingState = new PlayerAttackingState(this, _animator, _rb, toolCollder);
        var movementState = new PlayerMovementState(this, _animator, _rb);
        var wateringState = new PlayerWateringState(this, _animator, _rb);
        var weedingState = new PlayerWeedingState(this, _animator, _rb);
        
        _stateMachine.AddAnyTransition(attackingState, new FuncPredicate(() => IsAttacking));
        _stateMachine.AddAnyTransition(wateringState, new FuncPredicate(() => IsWatering));
        _stateMachine.AddAnyTransition(weedingState, new FuncPredicate(() => IsWeeding));
        
        _stateMachine.AddTransition(attackingState, movementState, new FuncPredicate(() => !IsAttacking));
        _stateMachine.AddTransition(wateringState, movementState, new FuncPredicate(() => !IsWatering));
        _stateMachine.AddTransition(weedingState, movementState, new FuncPredicate(() => !IsWeeding));
        
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

    public void Die()
    {
        Destroy(gameObject);
    }
}
