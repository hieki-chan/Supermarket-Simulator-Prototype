using System.Collections.Generic;

namespace Hieki.AI
{
    /// <summary>
    /// <see cref="Selector"/> is a <see cref="Node"/> that check its child nodes from left to right.
    /// If any of the child nodes return <see cref="NodeState.Success"/>/<see cref="NodeState.Running"/>, 
    /// <see cref="Selector"/> will immediately return <see cref="NodeState.Success"/>/<see cref="NodeState.Running"/>.
    /// If all of the child nodes return <see cref="NodeState.Failure"/>, <see cref="Selector"/> will return 
    /// <see cref="NodeState.Failure"/>.
    /// </summary>
    public sealed class Selector : CompositeNode
    {
        public Selector() : base() { }
        public Selector(List<Node> children) : base(children) { }

        protected sealed override NodeState Evaluate()
        {
            foreach (var child in children)
            {
                switch (child.Process())
                {
                    //immediately return running if any child nodes return running.
                    case NodeState.Running:
                        return currentState = NodeState.Running;
                    //immediately return success if any child nodes return success.
                    case NodeState.Success:
                        return currentState = NodeState.Success;
                    //move next to check whether all of the child nodes are failures.
                    case NodeState.Failure:
                        continue;
                    default:
                        continue;
                }
            }
            //return failure if entire child nodes return failure.
            return currentState = NodeState.Failure;
        }
    }
}
