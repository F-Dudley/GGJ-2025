using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BT.Strategies
{
    public class ActionStrategy : IStrategy
    {
        readonly Action func;

        public ActionStrategy(Action func)
        {
            this.func = func;
        }

        public ProcessStatus Process(Agent agent)
        {
            func.Invoke();
            return ProcessStatus.Success;
        }

        public void Reset()
        {
            throw new System.NotImplementedException();
        }
    }
}
