namespace Hieki.AI
{
    /// <summary>
    /// Leaft Node
    /// </summary>
    public class Leaf : Node
    {
        protected override NodeState Evaluate()
        {
            return Failure();
        }
    }
}
