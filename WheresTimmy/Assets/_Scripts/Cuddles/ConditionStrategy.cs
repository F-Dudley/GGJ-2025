using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BT.Strategies
{
    public class ConditionStrategy : IStrategy
    {
        readonly Func<bool> predicate;

        public ConditionStrategy(Func<bool> predicate)
        {
            this.predicate = predicate;
        }

        public ProcessStatus Process(Agent agent)
        {
            return predicate() ? ProcessStatus.Success : ProcessStatus.Failure;
        }

        public void Reset()
        {
            
        }
    }
}
