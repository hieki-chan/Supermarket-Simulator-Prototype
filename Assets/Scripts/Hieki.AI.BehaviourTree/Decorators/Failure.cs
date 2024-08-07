namespace Hieki.AI
{
    public class Failure : DecoratorNode
    {
        public Failure() : base(null) { }

        protected override NodeState Evaluate()
        {
            return currentState = NodeState.Failure;
        }
    }
}
