using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BT
{
    public class Sequential : Node
    {
        public Sequential(string name, int priority = 0) : base(name, priority)
        {

        }

        public override ProcessStatus Process(Agent agent)
        {
            if (currentChild < children.Count)
            {
                switch (children[currentChild].Process(agent))
                {
                    case ProcessStatus.Running:
                        return ProcessStatus.Running;

                    case ProcessStatus.Failure:
                        return ProcessStatus.Failure;

                    default:
                        currentChild++;
                        return currentChild == children.Count ? ProcessStatus.Success : ProcessStatus.Running;
                }
            }

            Reset();
            return ProcessStatus.Success;
        }
    }
}