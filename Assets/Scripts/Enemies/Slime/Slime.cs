using System;
using System.Collections;
using System.Collections.Generic;
using Enemies.FSM.StateMachine;
using Enemies.FSM.StateMachine.Predicates;
using Enemies.Slime.SlimeStates;
using Managers;
using UnityEngine;
using UnityEngine.AI;

namespace Enemies.Slime
{
    [RequireComponent(typeof(NavMeshAgent)), RequireComponent(typeof(Animator))]
    public class Slime : MonoBehaviour
    {
        [SerializeField] private EnemyData data;
        [SerializeField] private Collider2D chasingArea;
        [SerializeField] private Collider2D attackArea;
        
        private Dictionary<string, int> _chasingPriorities;         
        private Dictionary<string, int> _attackingPriorities;   
    
        private ContactFilter2D _contactFilter2D;
        private StateMachine _stateMachine;
        private NavMeshAgent _navMeshAgent;
        private Animator _animator;
        private bool isAlive;

        public bool isTakingDamage { get; set; }
        public Transform Target { get; private set; }

        private void Start()
        {
            _navMeshAgent = GetComponent<NavMeshAgent>();
            _animator = GetComponent<Animator>();
            isAlive = true;
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
            _navMeshAgent.speed = data.speed;
        }

        private void InitPriorities()
        {
            _chasingPriorities = new Dictionary<string, int>
            {
                {"Player", 1},
                {"Building", 0}
            };
        
            _attackingPriorities = new Dictionary<string, int>
            {
                {"Player", 1},
                {"Building", 0}
            };
        }
    
        private void InitStateMachine()
        {
            _stateMachine = new StateMachine();
        
            var wanderingState = new SlimeWanderingState(this, _navMeshAgent, _animator, 0.5f);
            var attackingState = new SlimeAttackingState(this, _navMeshAgent, _animator, attackArea, _contactFilter2D, data.damage);
            var friendlyState = new SlimeFriendlyState(this, _navMeshAgent, _animator);
            var dyingState = new SlimeDyingState(this, _navMeshAgent, _animator);
            var takingDamageState = new SlimeTakingDamageState(this, _navMeshAgent, _animator);

            Func<bool> isAbleToAct = () => isAlive && !isTakingDamage;
            _stateMachine.AddAnyTransition(wanderingState, new FuncPredicate(() => isAbleToAct() && !GameManager.Instance.dayNightManager.IsDay && Target is null));
            _stateMachine.AddAnyTransition(friendlyState, new FuncPredicate(() => isAbleToAct() && GameManager.Instance.dayNightManager.IsDay));
            _stateMachine.AddAnyTransition(attackingState, new FuncPredicate(() => isAbleToAct() && Target is not null));
            _stateMachine.AddAnyTransition(dyingState, new FuncPredicate(() => !isAlive));
            _stateMachine.AddAnyTransition(takingDamageState, new FuncPredicate(() => isTakingDamage && isAlive));
            
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

        public void Die()
        {
            isAlive = false;
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
            attackArea.Overlap(_contactFilter2D, colliders);

            Transform currentTarget = GetMaxPriorityTarget(colliders, _attackingPriorities);
            if (currentTarget is not null)
            {
                return currentTarget;
            }

            chasingArea.Overlap(_contactFilter2D, colliders);
            return GetMaxPriorityTarget(colliders, _chasingPriorities);
        }
        
        private Transform GetMaxPriorityTarget(List<Collider2D> colliders, Dictionary<string, int> priorities)
        {
            Transform currentTarget = null;
            int currentPriority = int.MaxValue;
            foreach (var col in colliders)
            {
                if (col.CompareTag("Player") || col.CompareTag("Building"))
                {
                    int priority = priorities[col.tag];
                    if (priority < currentPriority)
                    {
                        currentTarget = col.transform;
                        currentPriority = priority;
                    }
                }
            }

            return currentTarget;
        }
    }
}
