using System.Collections.Generic;

namespace Hieki.AI
{
    /// <summary>
    /// <see cref="Parallel"/> is a <see cref="Node"/> that check its child nodes from left to right.
    /// If all of the child nodes return <see cref="NodeState.Success"/>, <see cref="Parallel"/> will return 
    /// <see cref="NodeState.Success"/>.
    /// If all of the child nodes return <see cref="NodeState.Failure"/>, <see cref="Parallel"/> will return 
    /// <see cref="NodeState.Failure"/>.
    /// If any of the child nodes return <see cref="NodeState.Running"/>, <see cref="Parallel"/> will
    /// return <see cref="NodeState.Running"/>.
    /// </summary>
    public class Parallel : CompositeNode
    {
        bool isAnyChildRunning = false;
        int failedChildrenCount = 0;

        public Parallel() : base() { }
        public Parallel(List<Node> children) : base(children) { }

        protected override NodeState Evaluate()
        {
            foreach (var child in children)
            {
                switch (child.Process())
                {
                    //check if any child nodes return running.
                    case NodeState.Running:
                        isAnyChildRunning = true;
                        continue;
                    //move next to check whether all of the child nodes are success.
                    case NodeState.Success:
                        continue;
                    //immediately return failure if any child nodes return failure.
                    case NodeState.Failure:
                        failedChildrenCount++;
                        continue;
                    default:
                        continue;
                }
            }
            //if any child is running then return running, if all children fail then return failure, if all children success then return success
            return currentState = isAnyChildRunning ? NodeState.Running : failedChildrenCount == children.Count? NodeState.Failure : NodeState.Success;
        }
    }
}
