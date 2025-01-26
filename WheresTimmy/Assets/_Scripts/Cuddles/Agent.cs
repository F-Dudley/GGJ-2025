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
        [Header("Aggresion")]
        [SerializeField] private float _aggression = 0.0f;
        public float Aggression
        {
            get => _aggression;
            set
            {
                _aggression = Mathf.Clamp(value, minAggression, 100.0f);
                if (brokenJukeSource != null) brokenJukeSource.volume = Mathf.Clamp(_aggression / 100.0f, 0.0f, maxJukeBoxVolume);
            }
        }

        [SerializeField] private float minAggression = 0.0f;


        [SerializeField] private float aggressionGain = 4f;
        [SerializeField] private float aggressionDistance = 1.5f;
        [SerializeField] private float attackDistance = 1.0f;

        [SerializeField] private Player targetPlayer;


        [Header("Patrol Settings")]
        [SerializeField] private List<Transform> keyAgentLocations;


        private BehaviourTree tree;

        [Header("Nav Settings")]
        [SerializeField] private NavMeshAgent navAgent;
        [SerializeField] private float navDistance = 0.05f;

        [Header("Sensor")]
        [SerializeField] private float senseRadius;
        [SerializeField] private LayerMask playerMask;

        [Header("Misc")]
        [SerializeField] private float maxJukeBoxVolume = 0.6f;
        [SerializeField] private float biteClipScale = 1.0f;
        [SerializeField] private AudioSource brokenJukeSource;
        [SerializeField] private AudioClip biteAudio;

        public void Awake()
        {
            navAgent = GetComponent<NavMeshAgent>();

            minAggression = 0.0f;
            Aggression = 0.0f;

            brokenJukeSource.volume = 0.0f;

            BuildBehaviourTree();
        }

        private void Update()
        {
            SenseSurroundings();

            switch (tree.Process(this))
            {
                case ProcessStatus.Success:
                    tree.Reset();
                    break;

                default:
                    break;
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, senseRadius);

            Gizmos.color = Color.red;

            Gizmos.DrawWireSphere(transform.position, aggressionDistance);
        }

        private void BuildBehaviourTree()
        {
            tree = new BehaviourTree("Cuddles Behaviour");

            PrioritySelector prioSelector = new PrioritySelector("Agent Logic");

            // Aggression Step
            Sequential aggroSeq = new Sequential("Aggro Sequential", 50);

            aggroSeq.AddChild(new Leaf("IsAggresive", new ConditionStrategy(() => this.Aggression > 80.0f)));
            aggroSeq.AddChild(new Leaf("Chase Target", new ChaseStrategy()));
            aggroSeq.AddChild(new Leaf("Attack Target", new AttackStrategy()));

            prioSelector.AddChild(aggroSeq);

            // Aggresion Gain
            Sequential playerRangeAggro = new Sequential("Player Within Aggro Range", 30);
            playerRangeAggro.AddChild(new Leaf("Player Within Range", new ConditionStrategy(() => TargetDistance() < aggressionDistance)));
            playerRangeAggro.AddChild(new Leaf("Aggression Gain", new BuildAggroStrategy()));

            prioSelector.AddChild(playerRangeAggro);

            // Patrol Sequence
            Sequential patrolSeq = new Sequential("Patrol Sequence", 0);
            patrolSeq.AddChild(new Leaf("Has Key Locations", new ConditionStrategy(() => this.KeyLocationAmount != 0)));
            patrolSeq.AddChild(new Leaf("Patrol Strategy", new PatrolStrategy()));

            prioSelector.AddChild(patrolSeq);

            tree.AddChild(prioSelector);
        }

        #region Agent Traits

        public void AddMinAggresion(float amountToIncrease)
        {
            minAggression += amountToIncrease;
            Aggression = Mathf.Clamp(Aggression, minAggression, 100.0f);
        }

        public int KeyLocationAmount => keyAgentLocations.Count;

        public Vector3 GetKeyLocation(int index)
        {
            if (KeyLocationAmount <= 0)
                return Vector3.zero;

            return keyAgentLocations[index % keyAgentLocations.Count].position;
        }

        public void GainAggression()
        {
            Aggression += aggressionGain * Time.deltaTime;
        }

        public float TargetDistance()
        {
            if (targetPlayer != null)
            {
                float dist = Vector3.Distance(transform.position, targetPlayer.transform.position);
                Debug.Log("Target Dist: " + dist);

                return dist;
            }

            return float.MaxValue;
        }

        public bool WithinAggressionRange()
        {
            return TargetDistance() < aggressionDistance;
        }

        public bool WithinAttackRange()
        {
            return TargetDistance() < attackDistance;
        }

        public Player GetTarget() => targetPlayer;

        #endregion


        #region Nav Agent

        public void LookAt(Vector3 lookLocation)
        {
            lookLocation.y = navAgent.destination.y;

            transform.LookAt(lookLocation);
        }

        public void LookAtAgentPath()
        {
            if (!navAgent.hasPath)
                return;

            Quaternion q = Quaternion.LookRotation(navAgent.velocity);

            transform.rotation = Quaternion.Slerp(transform.rotation, q, 0.75f);
        }

        public void SetNavDestination(Vector3 targetPosition)
        {
            navAgent.SetDestination(targetPosition);
        }

        public void ResetNavDestination()
        {
            navAgent.ResetPath();
        }

        public bool PendingNavPath => navAgent.pathPending;

        public bool ArrivedAtDestination => navAgent.remainingDistance < navDistance;

        #endregion

        private void SenseSurroundings()
        {
            targetPlayer = null;
            Collider[] cols = Physics.OverlapSphere(transform.position, senseRadius, playerMask);
            if (cols.Length == 0)
                return;

            foreach (Collider col in cols)
            {
                if (col.TryGetComponent<Player>(out targetPlayer))
                {
                    if (targetPlayer != null)
                        break;
                }
            }
        }

        public void PlayBiteSound()
        {
            brokenJukeSource.PlayOneShot(biteAudio, biteClipScale);
        }
    }
}
