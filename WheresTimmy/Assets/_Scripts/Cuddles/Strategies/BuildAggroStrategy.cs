using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BT.Strategies
{
    public class BuildAggroStrategy : IStrategy
    {
        public BuildAggroStrategy()
        {

        }

        public ProcessStatus Process(Agent agent)
        {
            Player player = agent.GetTarget();
            if (player == null || !agent.WithinAggressionRange())
                return ProcessStatus.Failure;

            agent.ResetNavDestination();
            agent.LookAt(player.transform.position);

            agent.GainAggression();

            if (agent.Aggression >= 100.0f)
                return ProcessStatus.Success;

            return ProcessStatus.Running;
        }
    }
}