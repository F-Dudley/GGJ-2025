using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BT
{
    public interface IStrategy
    {
        ProcessStatus Process(ref Agent agent);
        void Reset();
    }

    public class Leaf : Node
    {
        readonly IStrategy strategy;

        public Leaf(string name, IStrategy strategy) : base(name)
        {
            this.strategy = strategy;
        }

        public override ProcessStatus Process(ref Agent agent) => strategy.Process(ref agent);

        public override void Reset() => strategy.Reset();
    }
}