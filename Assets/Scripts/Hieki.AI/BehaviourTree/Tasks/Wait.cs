using UnityEngine;

namespace Hieki.AI
{
    public class Wait : Node
    {
        protected readonly Node node;
        protected readonly float delay;
        protected float delayTimer = .0f;

        public Wait(Node node, float time)
        {
            this.node = node;
            this.delay = time;
        }

        protected override NodeState Evaluate()
        {
            delayTimer += Time.deltaTime;
            if (delayTimer >= delay)
            {
                delayTimer = 0;
                return currentState = node.Process();
            }

            node.Process();

            return currentState = NodeState.Running;
        }
    }

    public class WaitBefore : Wait
    {
        public WaitBefore(Node node, float time) : base(node, time) { }

        protected override NodeState Evaluate()
        {
            delayTimer += Time.deltaTime;
            if (delayTimer >= delay)
            {
                delayTimer = 0;
                return node.Process();

                //return currentState = NodeState.Success;
            }
            return currentState = NodeState.Running;
        }
    }

    public class WaitAfter : Wait
    {
        NodeState nodeState;

        public WaitAfter(Node node, float time) : base(node, time) 
        {
            nodeState = NodeState.Running;
        }

        protected override NodeState Evaluate()
        {
            if(delayTimer == 0)
            {
                nodeState = NodeState.Running;
            }
            switch (nodeState)
            {
                case NodeState.Running:
                    nodeState = node.Process();
                    return currentState = NodeState.Running;

                case NodeState.Success:
                    delayTimer += Time.deltaTime;
                    if (delayTimer >= delay)
                    {
                        delayTimer = 0;
                        return currentState = NodeState.Success;
                    }
                    return currentState = NodeState.Running;

                default:
                    return currentState = NodeState.Running;
            }
        }
    }

    public static class WaitExtensions
    {
        public static Wait Wait(this Node node, float time)
        {
            return new Wait(node, time);
        }

        public static WaitBefore WaitBefore(this Node node, float time)
        {
            return new WaitBefore(node, time);
        }

        public static WaitAfter WaitAfter(this Node node, float time)
        {
            return new WaitAfter(node, time);
        }
    }
}
