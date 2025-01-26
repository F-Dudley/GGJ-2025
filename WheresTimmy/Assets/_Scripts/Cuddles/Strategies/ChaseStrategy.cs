using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BT.Strategies
{
    public class ChaseStrategy : IStrategy
    {
        readonly float targetDistance;

        public ChaseStrategy(float targetDistance = 0.1f)
        {
            this.targetDistance = targetDistance;
        }

        public ProcessStatus Process(Agent agent)
        {


            return ProcessStatus.Running;
        }
    }
}