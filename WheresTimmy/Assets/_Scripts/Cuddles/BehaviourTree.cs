using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BT
{
    public class BehaviourTree : Node
    {
        public BehaviourTree(string name) : base(name) { }

        public override ProcessStatus Process(ref Agent agent)
        {
            while (currentChild < children.Count)
            {
                var status = children[currentChild].Process(ref agent);
                if (status != ProcessStatus.Success)
                    return status;

                currentChild++;
            }

            return ProcessStatus.Success;
        }
    }
}
