using System;

namespace Hieki.AI
{
    /// <summary>
    /// <see cref="Conditional"/> is a <see cref="Node"/> that only has 2 children. If conditional returns true, run child 1 else run child 2.
    /// If one of the children is <see cref="NodeState.Running"/>/<see cref="NodeState.Success"/>/<see cref="NodeState.Failure"/>, 
    /// <see cref="Conditional"/> will returns <see cref="NodeState.Running"/>/<see cref="NodeState.Success"/>/<see cref="NodeState.Failure"/>.
    /// Otherwise returns <see cref="NodeState.Invalid"/>
    /// </summary>
    public class Conditional : Node
    {
        Node condition, successChild, failureChild;

        public Conditional(Node condition, Node successChild, Node failureChild)
        {
            this.condition = condition;
            this.successChild = successChild;
            this.failureChild = failureChild;
        }

        protected override NodeState Evaluate()
        {
            switch(condition.Process())
            {
                case NodeState.Success:
                    failureChild.Release();
                    return currentState = successChild.Process();
                case NodeState.Failure:
                    successChild.Release();
                    return currentState = failureChild.Process();
                default:
                    return currentState = NodeState.Running;
            }
        }

        ///<example>
        /// Conditional basically is
        /// Selector
        ///     - Sequence
        ///         - Condition
        ///         - Then do Task A
        ///     - Else do Task B
        ///</example>
    }
}
