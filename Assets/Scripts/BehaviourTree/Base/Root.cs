namespace Hieki.AI
{
    /// <summary>
    /// Root of the behaviour tree
    /// </summary>
    public class Root : Node
    {
        Node child;
        public Root(Node node) : base() { child = node; }

        public override NodeState Evaluate()
        {
            return child.Evaluate();
        }
    }
}
