using System.Collections;
using System.Collections.Generic;
using Enemies.FSM.StateMachine;
using Enemies.FSM.StateMachine.Predicates;
using Enemies.Slime.SlimeStates;
using Managers;
using ScriptableObjects;
using ScriptableObjects.Enemies;
using UnityEngine;
using UnityEngine.AI;

namespace Enemies.Slime
{
    [RequireComponent(typeof(NavMeshAgent)), RequireComponent(typeof(Animator))]
    public class Slime : MonoBehaviour, IScarable
    {
        [SerializeField] private Collider2D chasingSightArea;
        [SerializeField] private Collider2D attackSightArea;
        [SerializeField] private Collider2D attackCollider;

        private Dictionary<string, int> _chasingPriorities;         
        private Dictionary<string, int> _attackingPriorities;   
    
        private SlimeTakingDamageState _takingDamageState;
        private ContactFilter2D _contactFilter2D;
        private StateMachine _stateMachine;
        private NavMeshAgent _navMeshAgent;
        private Animator _animator;
        private bool _isAlive;
        
        public bool NeedRewardForDying { get; set; }
        [field: SerializeField] public EnemyData Data { get; private set; }
        public bool isTakingDamage { get; set; }
        public Transform Target { get; private set; }

        private Dictionary<IScary, int> _activeScareSources = new();
        private int _totalScare;
        
        private void Start()
        {
            _navMeshAgent = GetComponent<NavMeshAgent>();
            _animator = GetComponent<Animator>();
            _isAlive = true;
            InitNavMeshAgent();
            _contactFilter2D.NoFilter();
            InitPriorities();
            InitStateMachine();
        
            StartCoroutine(FindTarget());
        }

        private void InitNavMeshAgent()
        {
            _navMeshAgent.updateRotation = false;
            _navMeshAgent.updateUpAxis = false;
            _navMeshAgent.speed = Data.speed;
        }

        private void InitPriorities()
        {
            _chasingPriorities = new Dictionary<string, int>();
            foreach (var priority in Data.chasingPriorities)
            {
                _chasingPriorities[priority.colTag] = priority.priority;
            }
            
            _attackingPriorities = new Dictionary<string, int>();
            foreach (var priority in Data.attackingPriorities)
            {
                _attackingPriorities[priority.colTag] = priority.priority;
            }
        }

        private void InitStateMachine()
        {
            _stateMachine = new StateMachine();
        
            var wanderingState = new SlimeWanderingState(this, _navMeshAgent, _animator, 0.5f);
            var attackingState = new SlimeAttackingState(this, _navMeshAgent, _animator, attackCollider, _contactFilter2D);
            var friendlyState = new SlimeFriendlyState(this, _navMeshAgent, _animator);
            var dyingState = new SlimeDyingState(this, _navMeshAgent, _animator);
            var slimeScaredState = new SlimeScaredState(this, _navMeshAgent, _animator);
            _takingDamageState = new SlimeTakingDamageState(this, _navMeshAgent, _animator);

            bool IsAbleToAct() => _isAlive && !isTakingDamage && _totalScare <= 0;
            _stateMachine.AddAnyTransition(wanderingState, new FuncPredicate(() => IsAbleToAct() && !GameManager.Instance.dayNightManager.IsDay && Target is null));
            _stateMachine.AddAnyTransition(friendlyState, new FuncPredicate(() => IsAbleToAct() && GameManager.Instance.dayNightManager.IsDay));
            _stateMachine.AddAnyTransition(attackingState, new FuncPredicate(() => IsAbleToAct() && Target is not null));
            _stateMachine.AddAnyTransition(_takingDamageState, new FuncPredicate(() => isTakingDamage && _isAlive));
            _stateMachine.AddAnyTransition(slimeScaredState, new FuncPredicate(() => _isAlive && !isTakingDamage && _totalScare > 0));
            _stateMachine.AddAnyTransition(dyingState, new FuncPredicate(() => !_isAlive));
            
            _stateMachine.StartEntryState(wanderingState);
        }

        private void Update()
        {
            _stateMachine.Update();
        }

        private void FixedUpdate()
        {
            _stateMachine.FixedUpdate();
        }
    
        public void AnimationEvent(AnimationEvent animationEvent)
        {
            _stateMachine.AnimationEvent(animationEvent);
        }

        public void TakeDamage(HitInfo? hitInfo, int _)
        {
            if (!hitInfo.HasValue)
            {
                return;
            }
            
            _takingDamageState.HitInfo = hitInfo.Value;
            isTakingDamage = true;
        }
        
        public void Die()
        {
            _isAlive = false;
        }
    
        private IEnumerator FindTarget()
        {
            while (gameObject.activeInHierarchy)
            {
                Target = GetTarget();
                yield return new WaitForSeconds(0.5f);
            }
        }
        
        private Transform GetTarget()
        {
            List<Collider2D> colliders = new List<Collider2D>();
            attackSightArea.Overlap(_contactFilter2D, colliders);

            Transform currentTarget = GetMaxPriorityTarget(colliders, _attackingPriorities);
            if (currentTarget is not null)
            {
                return currentTarget;
            }

            chasingSightArea.Overlap(_contactFilter2D, colliders);
            return GetMaxPriorityTarget(colliders, _chasingPriorities);
        }
        
        private Transform GetMaxPriorityTarget(List<Collider2D> colliders, Dictionary<string, int> priorities)
        {
            Transform currentTarget = null;
            int currentPriority = int.MinValue;
            float currentDistance = int.MaxValue;
            foreach (var col in colliders)
            {
                foreach (var prioritiesKey in priorities.Keys)
                {
                    if (!col.CompareTag(prioritiesKey))
                    {
                        continue;
                    }
                    
                    
                    int priority = priorities[col.tag];
                    if (priority > currentPriority)
                    {
                        currentTarget = col.transform;
                        currentPriority = priority;
                    } else if (priority == currentPriority && 
                               Vector2.SqrMagnitude(transform.position - col.transform.position) < currentDistance)
                    {
                        currentTarget = col.transform;
                        currentDistance = Vector2.SqrMagnitude(transform.position - col.transform.position);
                    }
                }
            }

            return currentTarget;
        }

        private void OnDestroy()
        {
            if (!NeedRewardForDying)
            {
                return;
            }
            
            for (int i = 0; i < Random.Range(1, 4); ++i)
            {
                Vector2 randomPosition = (Vector2)transform.position + Random.insideUnitCircle * 0.5f;
                Instantiate(Data.dropItem, randomPosition, Quaternion.identity);
            }
        }
        
        public void UpdateScareFromSource(IScary source, int amount)
        {
            _activeScareSources[source] = amount;
            _totalScare = CalculateTotalScare();
        }

        public void RemoveScareFromSource(IScary source)
        {
            if (_activeScareSources.Remove(source))
            {
                _totalScare = CalculateTotalScare();
            }
        }
        
        public IScary GetStrongestScareSource()
        {
            IScary strongestSource = null;
            int maxScare = 0;
            float currentDistance = int.MaxValue;
            
            foreach (var pair in _activeScareSources)
            {
                if (pair.Value > maxScare)
                {
                    maxScare = pair.Value;
                    strongestSource = pair.Key;
                    currentDistance = Vector2.SqrMagnitude(transform.position - pair.Key.GetTransform().position);
                } else if (pair.Value == maxScare && 
                           Vector2.SqrMagnitude(transform.position - pair.Key.GetTransform().position) < currentDistance)
                {
                    maxScare = pair.Value;
                    strongestSource = pair.Key;
                    currentDistance = Vector2.SqrMagnitude(transform.position - pair.Key.GetTransform().position);
                }
            }
            
            return strongestSource;
        }
        
        private int CalculateTotalScare()
        {
            int totalScare = 0;
            foreach (var scareSource in _activeScareSources)
            {
                totalScare += scareSource.Value;
            }

            return totalScare;
        }
    }
}
