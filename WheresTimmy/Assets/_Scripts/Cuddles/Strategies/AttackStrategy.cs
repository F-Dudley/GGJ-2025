using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BT.Strategies
{
    public class AttackStrategy : IStrategy
    {
        public AttackStrategy()
        {

        }

        public ProcessStatus Process(Agent agent)
        {
            if (!agent.WithinAttackRange())
                return ProcessStatus.Failure;

            Player target = agent.GetTarget();

            target.AddHealth(-1);
            agent.Aggression = 0.0f;

            return ProcessStatus.Success;
        }
    }
}