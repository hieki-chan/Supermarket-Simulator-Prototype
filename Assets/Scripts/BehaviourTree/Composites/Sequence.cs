using System.Collections.Generic;

namespace Hieki.AI
{
    /// <summary>
    /// <see cref="Sequence"/> is a <see cref="Node"/> that check its child nodes from left to right.
    /// If all of the child nodes return <see cref="NodeState.Success"/>, <see cref="Sequence"/> will return 
    /// <see cref="NodeState.Success"/>.
    /// If any of the child nodes return <see cref="NodeState.Failure"/>, <see cref="Sequence"/> will
    /// immediately return <see cref="NodeState.Failure"/>.
    /// Otherwise return <see cref="NodeState.Running"/>
    /// </summary>
    public sealed class Sequence : CompositeNode
    {
        bool isAnyChildRunning = false;

        public Sequence() : base() { }
        public Sequence(List<Node> children) : base(children) { }

        protected sealed override NodeState Evaluate()
        {
            foreach (var child in children)
            {
                switch (child.Process())
                {
                    //return running if any child nodes return running.
                    case NodeState.Running:
                        isAnyChildRunning = true;
                        continue;
                    //move next to check whether all of the child nodes are success.
                    case NodeState.Success:
                        continue;
                    //immediately return failure if any child nodes return failure.
                    case NodeState.Failure:
                        return currentState = NodeState.Failure;
                    default:
                        continue;
                }
            }
            //if no child nodes return running or failure then return success. It means entire node return success.
            return currentState = isAnyChildRunning ? NodeState.Running : NodeState.Success;
        }
    }
}
