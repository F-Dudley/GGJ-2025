using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BT
{
    public class Selector : Node
    {


        public Selector(string name, int priority = 0) : base(name, priority)
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

                    case ProcessStatus.Success:
                        return ProcessStatus.Success;
                    default:
                        currentChild++;
                        return ProcessStatus.Running;
                }
            }

            Reset();
            return ProcessStatus.Failure;
        }
    }

    public class PrioritySelector : Selector
    {
        List<Node> sortedChildren;
        List<Node> SortedChildren => sortedChildren ??= SortChildren();

        public PrioritySelector(string name, int priority = 0) : base(name, priority) { }

        public override void Reset()
        {
            base.Reset();
            sortedChildren = null;
        }

        public override ProcessStatus Process(Agent agent)
        {
            foreach (var child in SortedChildren)
            {
                switch (child.Process(agent))
                {
                    case ProcessStatus.Running:
                        return ProcessStatus.Running;
                    case ProcessStatus.Success:
                        return ProcessStatus.Success;
                    default:
                        continue;
                }
            }

            return ProcessStatus.Failure;
        }

        protected virtual List<Node> SortChildren() => children.OrderByDescending(child => child.priority).ToList();
    }

    public class RandomSelector : PrioritySelector
    {
        public RandomSelector(string name, int priority = 0) : base(name, priority)
        {

        }

        protected override List<Node> SortChildren()
        {
            children.Shuffle();
            return children;
        }
    }
}


public static class IListExtensions
{
    /// <summary>
    /// Shuffles the element order of the specified list.
    /// </summary>
    public static void Shuffle<T>(this IList<T> ts)
    {
        var count = ts.Count;
        var last = count - 1;
        for (var i = 0; i < last; ++i)
        {
            var r = UnityEngine.Random.Range(i, count);
            var tmp = ts[i];
            ts[i] = ts[r];
            ts[r] = tmp;
        }
    }
}