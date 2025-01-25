using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BT
{
    public enum ProcessStatus { Success, Failure, Running }

    public class Node
    {
        // Node
        public readonly string name;
        public readonly int priority;

        // Children
        public readonly List<Node> children = new();
        protected int currentChild;

        public Node(string name = "DefaultNode")
        {
            this.name = name;
        }

        public void AddChild(Node child) => children.Add(child);

        public virtual ProcessStatus Process(ref Agent agent) => children[currentChild].Process(ref agent);

        public virtual void Reset()
        {
            currentChild = 0;

            foreach (var child in children)
            {
                child.Reset();
            }
        }
    }
}
