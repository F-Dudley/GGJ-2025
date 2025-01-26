using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BT.Strategies
{
    public class ChaseStrategy : IStrategy
    {
        public ChaseStrategy()
        {

        }

        public ProcessStatus Process(Agent agent)
        {
            Player target = agent.GetTarget();

            if (target == null || agent.Aggression <= 40.0f)
                return ProcessStatus.Failure;

            Debug.Log("CHASING THE PLAYER");

            if (agent.WithinAttackRange())
                return ProcessStatus.Success;

            agent.SetNavDestination(target.transform.position);
            agent.LookAt(target.transform.position);

            return ProcessStatus.Running;
        }
    }
}