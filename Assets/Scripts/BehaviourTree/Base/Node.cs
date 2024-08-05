using System.Runtime.CompilerServices;

namespace Hieki.AI
{
    public abstract class Node
    {
        /// <summary>
        /// current state of the node.
        /// </summary>
        public NodeState currentState
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            protected set;
        } 
            = NodeState.Unknow;

        public Node()
        {
            this.currentState = NodeState.Unknow;
        }

        /*public bool entered { get; set; } = false;

        /// <summary>
        /// Enters the node
        /// </summary>
        public void Enter()
        {
            if (entered == false)
                OnEnter();
            entered = true;
        }

        /// <summary>
        /// Exits the node.
        /// </summary>
        public void Exit()
        {
            entered = false;
        }

        /// <summary>
        /// Called when a enter on the node.
        /// </summary>
        protected virtual void OnEnter()
        {

        }*/


        public NodeState Process()
        {
            if(currentState == NodeState.Unknow)
            {
                OnPerform();
            }

            return Evaluate();
        }

        protected virtual void OnPerform()
        {

        }

        public virtual void Release()
        {
            currentState = NodeState.Unknow;
        }


        /// <summary>
        /// Evaluates the node
        /// </summary>
        /// <returns></returns>
        protected abstract NodeState Evaluate();


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected NodeState Success() => currentState = NodeState.Success;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected NodeState Running() => currentState = NodeState.Running;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected NodeState Failure() => currentState = NodeState.Failure;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]

        public static implicit operator bool(Node node)
        {
            return node is not null;
        }
    }
}
