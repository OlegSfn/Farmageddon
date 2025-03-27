using System.Collections;
using System.Collections.Generic;
using Enemies.FSM.StateMachine;
using Enemies.FSM.StateMachine.Predicates;
using Enemies.Slime.SlimeStates;
using Managers;
using ScriptableObjects.Enemies;
using UnityEngine;
using UnityEngine.AI;

namespace Enemies.Slime
{
    /// <summary>
    /// Main slime enemy class, handles movement, targeting, and state transitions
    /// </summary>
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
        
        /// <summary>
        /// Flag indicating whether this slime should drop rewards when killed
        /// </summary>
        public bool NeedRewardForDying { get; set; }
        
        /// <summary>
        /// Enemy data from ScriptableObject containing configuration settings
        /// </summary>
        [field: SerializeField] public EnemyData Data { get; private set; }
        
        /// <summary>
        /// Flag indicating whether the slime is currently taking damage
        /// </summary>
        public bool isTakingDamage { get; set; }
        
        /// <summary>
        /// Current target the slime is trying to attack
        /// </summary>
        public Transform Target { get; private set; }

        /// <summary>
        /// Dictionary of active fear sources affecting this slime
        /// </summary>
        private Dictionary<IScary, int> _activeScareSources = new();
        
        /// <summary>
        /// Total fear level from all sources
        /// </summary>
        private int _totalScare;
        
        /// <summary>
        /// Initialize components and start target finding process
        /// </summary>
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

        /// <summary>
        /// Configure NavMeshAgent parameters for 2D movement
        /// </summary>
        private void InitNavMeshAgent()
        {
            _navMeshAgent.updateRotation = false;
            _navMeshAgent.updateUpAxis = false;
            _navMeshAgent.speed = Data.speed;
        }

        /// <summary>
        /// Initialize target priority dictionaries for chasing and attacking
        /// </summary>
        private void InitPriorities()
        {
            _chasingPriorities = CreatePriorityDictionary(Data.chasingPriorities);
            _attackingPriorities = CreatePriorityDictionary(Data.attackingPriorities);
        }
        
        /// <summary>
        /// Creates a priority dictionary from the priority data in the configuration
        /// </summary>
        /// <param name="priorities">Priority data from configuration</param>
        /// <returns>
        /// A dictionary with mapped collider tags to priority values from priorities
        /// </returns>
        private Dictionary<string, int> CreatePriorityDictionary(IEnumerable<EnemyData.PriorityMap> priorities)
        {
            Dictionary<string, int> priorityDict = new();
            foreach (var priority in priorities)
            {
                priorityDict[priority.colTag] = priority.priority;
            }

            return priorityDict;
        }

        /// <summary>
        /// Initialize the state machine with all possible slime states and their transitions
        /// </summary>
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

        /// <summary>
        /// Proxies the state machine update call
        /// </summary>
        private void Update()
        {
            _stateMachine.Update();
        }

        /// <summary>
        /// Proxies the state machine fixed update call
        /// </summary>
        private void FixedUpdate()
        {
            _stateMachine.FixedUpdate();
        }
    
        /// <summary>
        /// Proxies the state machine animation event call
        /// </summary>
        /// <param name="animationEvent">Event data from the animation</param>
        public void AnimationEvent(AnimationEvent animationEvent)
        {
            _stateMachine.AnimationEvent(animationEvent);
        }

        /// <summary>
        /// Process damage taken from external sources
        /// </summary>
        /// <param name="hitInfo">Information about the hit that caused damage</param>
        /// <param name="_">Unused damage amount parameter</param>
        public void TakeDamage(HitInfo? hitInfo, int _)
        {
            if (!hitInfo.HasValue)
            {
                return;
            }
            
            _takingDamageState.HitInfo = hitInfo.Value;
            isTakingDamage = true;
        }
        
        /// <summary>
        /// Mark the slime as dead, triggering transition to death state
        /// </summary>
        public void Die()
        {
            _isAlive = false;
        }
    
        /// <summary>
        /// Periodically scans for potential targets within detection range
        /// </summary>
        private IEnumerator FindTarget()
        {
            while (gameObject.activeInHierarchy)
            {
                Target = GetTarget();
                yield return new WaitForSeconds(0.5f);
            }
        }
        
        /// <summary>
        /// Attempts to find a target in attack range, then in chase range if none found
        /// </summary>
        /// <returns>Transform of the highest priority target, or null if none found</returns>
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
        
        /// <summary>
        /// Finds the highest priority target based on priority settings and distance
        /// </summary>
        /// <param name="colliders">List of colliders in detection range</param>
        /// <param name="priorities">Priority dictionary mapping tags to priority values</param>
        /// <returns>Transform of the highest priority target, or null if none found</returns>
        private Transform GetMaxPriorityTarget(List<Collider2D> colliders, Dictionary<string, int> priorities)
        {
            Transform currentTarget = null;
            int currentPriority = int.MinValue;
            float currentDistance = int.MaxValue;
            
            foreach (var col in colliders)
            {
                if (!priorities.TryGetValue(col.tag, out int priority))
                {
                    continue;
                }
                
                // Update target if this one has higher priority or same priority but closer
                UpdateTargetIfBetter(col.transform, priority, ref currentTarget, ref currentPriority, ref currentDistance);
            }

            return currentTarget;
        }
        
        /// <summary>
        /// Updates the current target if the new candidate is better (higher priority or same priority but closer)
        /// </summary>
        /// <param name="candidateTransform">Transform of the potential new target</param>
        /// <param name="candidatePriority">Priority value of the potential new target</param>
        /// <param name="currentTarget">Reference to current best target</param>
        /// <param name="currentPriority">Reference to current highest priority</param>
        /// <param name="currentDistance">Reference to current closest distance</param>
        private void UpdateTargetIfBetter(
            Transform candidateTransform,
            int candidatePriority,
            ref Transform currentTarget,
            ref int currentPriority,
            ref float currentDistance)
        {
            float candidateDistance = Vector2.SqrMagnitude(transform.position - candidateTransform.position);
            
            if (candidatePriority > currentPriority ||
                (candidatePriority == currentPriority && candidateDistance < currentDistance))
            {
                currentTarget = candidateTransform;
                currentPriority = candidatePriority;
                currentDistance = candidateDistance;
            }
        }

        /// <summary>
        /// Checks for the need to drop rewards when the slime is destroyed
        /// </summary>
        private void OnDestroy()
        {
            if (!NeedRewardForDying)
            {
                return;
            }
            
            DropRewards();
        }
        
        /// <summary>
        /// Spawns random number of drop items at slightly randomized positions
        /// </summary>
        private void DropRewards()
        {
            int dropCount = Random.Range(1, 4);
            for (int i = 0; i < dropCount; ++i)
            {
                Vector2 randomPosition = (Vector2)transform.position + Random.insideUnitCircle * 0.5f;
                Instantiate(Data.dropItem, randomPosition, Quaternion.identity);
            }
        }
        
        /// <summary>
        /// Updates the fear level from a specific source and recalculates total fear
        /// </summary>
        /// <param name="source">The source causing fear</param>
        /// <param name="amount">The amount of fear (higher values = more fear)</param>
        public void UpdateScareFromSource(IScary source, int amount)
        {
            _activeScareSources[source] = amount;
            _totalScare = CalculateTotalScare();
        }

        /// <summary>
        /// Removes a fear source and recalculates total fear
        /// </summary>
        /// <param name="source">The fear source to remove</param>
        public void RemoveScareFromSource(IScary source)
        {
            if (_activeScareSources.Remove(source))
            {
                _totalScare = CalculateTotalScare();
            }
        }
        
        /// <summary>
        /// Returns the source causing the most fear (higher scare or same scare but closer)
        /// </summary>
        /// <returns>The strongest fear source, or null if none active</returns>
        public IScary GetStrongestScareSource()
        {
            IScary strongestSource = null;
            int maxScare = 0;
            float currentDistance = int.MaxValue;
            
            foreach (var pair in _activeScareSources)
            {
                float sourceDistance = Vector2.SqrMagnitude(transform.position - pair.Key.GetTransform().position);
                UpdateScareSourceIfStronger(pair.Key, pair.Value, sourceDistance,
                    ref strongestSource, ref maxScare, ref currentDistance);
            }
            
            return strongestSource;
        }
        
        /// <summary>
        /// Updates the strongest fear source if the new source is scarier or closer
        /// </summary>
        /// <param name="source">The candidate fear source</param>
        /// <param name="scareAmount">The fear amount from this source</param>
        /// <param name="distance">The distance to this source</param>
        /// <param name="strongestSource">Reference to current strongest source</param>
        /// <param name="maxScare">Reference to current maximum fear value</param>
        /// <param name="currentDistance">Reference to current closest distance</param>
        private void UpdateScareSourceIfStronger(
            IScary source,
            int scareAmount,
            float distance,
            ref IScary strongestSource,
            ref int maxScare,
            ref float currentDistance)
        {
            if (scareAmount > maxScare ||
                (scareAmount == maxScare && distance < currentDistance))
            {
                strongestSource = source;
                maxScare = scareAmount;
                currentDistance = distance;
            }
        }
        
        /// <summary>
        /// Calculates the total fear level from all active sources
        /// </summary>
        /// <returns>The sum of all fear values</returns>
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
