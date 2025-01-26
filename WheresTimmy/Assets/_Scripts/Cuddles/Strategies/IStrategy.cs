using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BT.Strategies
{
    public interface IStrategy
    {
        ProcessStatus Process(Agent agent);

        void Reset() { }
    }
}
