namespace Hieki.AI
{
    public class Success : DecoratorNode
    {
        public Success() : base(null) { }

        public override NodeState Evaluate()
        {
            return currentState = NodeState.Success;
        }
    }
}
