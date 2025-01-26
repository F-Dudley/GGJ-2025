using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BT
{
    public class BehaviourTree : Node
    {
        public BehaviourTree(string name) : base(name) { }

        public override ProcessStatus Process(Agent agent)
        {
            while (currentChild < children.Count)
            {
                var status = children[currentChild].Process(agent);
                if (status != ProcessStatus.Success)
                    return status;

                currentChild++;
            }

            return ProcessStatus.Success;
        }
    }
}
