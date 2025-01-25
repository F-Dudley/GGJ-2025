using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

namespace BT
{
    public class Agent : MonoBehaviour
    {

        [Header("Agent Settings")]
        [SerializeField] private float speed = 2.0f;
        [SerializeField] private float agression = 0.1f;


        [SerializeField] private List<Transform> keyAgentLocations;


        [Header("Nav Settings")]
        [SerializeField] private NavMeshAgent navAgent;

        public void Awake()
        {
            navAgent = GetComponent<NavMeshAgent>();
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

        public void SetNavDestination(Vector3 targetPosition)
        {
            navAgent.SetDestination(targetPosition);
        }

        public bool PendingNavPath => navAgent.pathPending;

        public bool ArrivedAtDestination => PendingNavPath && navAgent.remainingDistance < 0.1f;

        #endregion
    }
}