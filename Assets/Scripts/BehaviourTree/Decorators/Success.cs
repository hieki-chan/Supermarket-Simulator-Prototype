namespace Hieki.AI
{
    public class Success : DecoratorNode
    {
        public Success() : base(null) { }

        protected override NodeState Evaluate()
        {
            return currentState = NodeState.Success;
        }
    }
}
