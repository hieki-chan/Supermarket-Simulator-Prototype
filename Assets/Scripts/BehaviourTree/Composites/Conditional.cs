/*using System;

namespace ProjectN.System
{
    /// <summary>
    /// <see cref="Conditional"/> is a <see cref="Node"/> that only has 2 children. If conditional returns true, run child 1 else run child 2.
    /// If one of the children is <see cref="NodeState.Running"/>/<see cref="NodeState.Success"/>/<see cref="NodeState.Failure"/>, 
    /// <see cref="Conditional"/> will returns <see cref="NodeState.Running"/>/<see cref="NodeState.Success"/>/<see cref="NodeState.Failure"/>.
    /// Otherwise returns <see cref="NodeState.Invalid"/>
    /// </summary>
    [CreateTreeNodeMenu("Composites/Conditional")]
    public class Conditional : Node
    {
        readonly Func<Node, bool> condition;
        NodeState child1State, child2State;

        public Conditional() : base() { }
        public Conditional(Node successChild, Node failureChild, Func<Node, bool> condition) : base()
        { 
            children.Add(successChild);
            children.Add(failureChild);
            this.condition = condition; 
        }

        public override NodeState Evaluate()
        {
            child1State = child2State = NodeState.Invalid;
            if (condition(this))
            {
                if (children[0])
                    child1State = children[0].Evaluate();
            }
            else 
            {
                if (children[0])
                    child2State = children[0].Evaluate();
            }
            //cases:
            //- one is success,
            //- one is running,
            //- one is failure,
            //- both childs are null or invalid.
            return currentState = 
                (child1State == NodeState.Success || child2State == NodeState.Success) ? NodeState.Success :
                (child1State == NodeState.Running && child2State == NodeState.Running) ? NodeState.Running :
                (child1State == NodeState.Failure && child2State == NodeState.Failure) ? NodeState.Failure : NodeState.Invalid;
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
*/