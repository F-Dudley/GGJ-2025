using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BT.Strategies
{
    public class PatrolStrategy : IStrategy
    {
        int currentIndex = 0;
        bool isPathCalculated;

        public PatrolStrategy()
        {

        }

        public ProcessStatus Process(Agent agent)
        {
            if (agent.KeyLocationAmount == 0)
                return ProcessStatus.Failure;

            if (currentIndex == agent.KeyLocationAmount)
                return ProcessStatus.Success;


            Vector3 target = agent.GetKeyLocation(currentIndex);
            agent.SetNavDestination(target);
            agent.LookAt(target);

            if (isPathCalculated && agent.ArrivedAtDestination)
            {
                currentIndex += 1;
                currentIndex %= agent.KeyLocationAmount;

                isPathCalculated = false;
            }

            if (agent.PendingNavPath)
            {
                isPathCalculated = true;
            }


            return ProcessStatus.Running;
        }

        public void Reset() => currentIndex = 0;
    }
}