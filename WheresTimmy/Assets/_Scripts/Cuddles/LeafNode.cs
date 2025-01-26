using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BT
{
    public class Leaf : Node
    {
        readonly Strategies.IStrategy strategy;

        public Leaf(string name, Strategies.IStrategy strategy, int priority = 0) : base(name, priority)
        {
            this.strategy = strategy;
        }

        public override ProcessStatus Process(Agent agent) => strategy.Process(agent);

        public override void Reset() => strategy.Reset();
    }
}