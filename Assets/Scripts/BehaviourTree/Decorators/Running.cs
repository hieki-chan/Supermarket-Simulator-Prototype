namespace Hieki.AI
{
    public class Running : DecoratorNode
    {
        public Running() : base(null) { }

        public override NodeState Evaluate()
        {
            return currentState = NodeState.Running;
        }
    }
}
