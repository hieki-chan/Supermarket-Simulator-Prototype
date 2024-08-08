namespace Hieki.AI
{
    public class Inverter : DecoratorNode
    {
        public Inverter(Node child) : base(child) { }
        protected override NodeState Evaluate() => currentState = child.Process() switch
        {
            NodeState.Running => NodeState.Running,
            NodeState.Success => NodeState.Failure,
            NodeState.Failure => NodeState.Success,
            NodeState.Unknow => NodeState.Unknow,
            _ => NodeState.Failure,
        };
    }

    public static class InverterExtensions
    {
        /// <summary>
        /// Inverts the Node
        /// </summary>
        public static Inverter Invert<T>(this T node) where T : Node
        {
            return new Inverter(node);
        }
    }
}
