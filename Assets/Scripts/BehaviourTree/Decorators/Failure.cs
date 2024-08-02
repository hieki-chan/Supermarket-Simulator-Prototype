namespace Hieki.AI
{
    public class Failure : DecoratorNode
    {
        public Failure() : base(null) { }

        public override NodeState Evaluate()
        {
            return currentState = NodeState.Failure;
        }
    }
}
