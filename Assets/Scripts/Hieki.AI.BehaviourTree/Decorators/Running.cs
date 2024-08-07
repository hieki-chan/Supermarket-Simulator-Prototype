namespace Hieki.AI
{
    public class Running : DecoratorNode
    {
        public Running() : base(null) { }

        protected override NodeState Evaluate()
        {
            return currentState = NodeState.Running;
        }
    }
}
