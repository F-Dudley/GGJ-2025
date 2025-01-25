using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Video;

namespace BT
{
    public class PatrolStrategy : IStrategy
    {
        int currentIndex;

        public PatrolStrategy()
        {

        }

        public ProcessStatus Process(ref Agent agent)
        {
            if (agent.KeyLocationAmount == 0)
                return ProcessStatus.Failure;

            if (currentIndex == agent.KeyLocationAmount)
                return ProcessStatus.Success;

            Vector3 target = agent.GetKeyLocation(currentIndex);
            agent.SetNavDestination(target);

            return ProcessStatus.Running;
        }

        public void Reset() => currentIndex = 0;
    }
}