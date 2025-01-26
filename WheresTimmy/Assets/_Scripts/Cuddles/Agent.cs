using System.Collections.Generic;
using System.Linq;
using BT.Strategies;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

namespace BT
{
    public class Agent : MonoBehaviour
    {

        [Header("Agent Settings")]
        [SerializeField] private float speed = 2.0f;


        [Header("Aggresion")]
        [SerializeField] private float _aggression = 0.0f;
        float Aggression
        {
            get => _aggression;
            set
            {
                _aggression = value;
            }
        }

        [SerializeField] private float aggressionGain = 4f;



        [Header("Patrol Settings")]
        [SerializeField] private List<Transform> keyAgentLocations;


        private BehaviourTree tree;

        [Header("Nav Settings")]
        [SerializeField] private NavMeshAgent navAgent;
        [SerializeField] private float navDistance = 0.05f;

        public void Awake()
        {
            navAgent = GetComponent<NavMeshAgent>();

            BuildBehaviourTree();
        }

        private void Update()
        {
            tree.Process(this);

            Debug.Log("Distance From Target: " + navAgent.remainingDistance);
        }

        private void BuildBehaviourTree()
        {
            tree = new BehaviourTree("Cuddles Behaviour");

            PrioritySelector prioSelector = new PrioritySelector("Agent Logic");

            // Aggression Step
            Sequential aggroSeq = new Sequential("Aggro Sequential", 50);

            aggroSeq.AddChild(new Leaf("IsAggresive", new ConditionStrategy(() => this.Aggression > 80.0f)));
            aggroSeq.AddChild(new Leaf("Chase Player", new ChaseStrategy()));

            // Aggresion Gain
            Sequential playerRangeAggro = new Sequential("Player Within Aggro Range", 30);
            playerRangeAggro.AddChild(new Leaf("Player Within Range", new ConditionStrategy(() => false)));
            playerRangeAggro.AddChild(new Leaf("Aggression Gain", new ActionStrategy(() =>
            {
                navAgent.ResetPath();
                Aggression += aggressionGain * Time.deltaTime;
            })));

            prioSelector.AddChild(playerRangeAggro);

            // Patrol Sequence
            Sequential patrolSeq = new Sequential("Patrol Sequence", 10);
            patrolSeq.AddChild(new Leaf("Has Key Locations", new ConditionStrategy(() => this.KeyLocationAmount != 0)));
            patrolSeq.AddChild(new Leaf("Patrol Strategy", new PatrolStrategy()));

            prioSelector.AddChild(patrolSeq);

            tree.AddChild(prioSelector);
        }

        #region Agent Traits

        public int KeyLocationAmount => keyAgentLocations.Count;

        public Vector3 GetKeyLocation(int index)
        {
            if (KeyLocationAmount <= 0)
                return Vector3.zero;

            return keyAgentLocations[index % keyAgentLocations.Count].position;
        }

        #endregion


        #region Nav Agent

        public void LookAt(Vector3 lookLocation)
        {
            lookLocation.y = navAgent.destination.y;

            transform.LookAt(lookLocation);
        }

        public void SetNavDestination(Vector3 targetPosition)
        {
            navAgent.SetDestination(targetPosition);
        }

        public bool PendingNavPath => navAgent.pathPending;

        public bool ArrivedAtDestination => navAgent.remainingDistance < navDistance;
    }

    #endregion
}
