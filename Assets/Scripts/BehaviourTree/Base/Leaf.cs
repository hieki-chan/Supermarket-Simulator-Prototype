namespace Hieki.AI
{
    /// <summary>
    /// Leaft Node
    /// </summary>
    public class Leaf : Node
    {
        public override NodeState Evaluate()
        {
            return Failure();
        }
    }
}
