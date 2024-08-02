﻿namespace Hieki.AI
{
    public class Inverter : DecoratorNode
    {
        public Inverter(Node child) : base(child) { }
        public override NodeState Evaluate() => currentState = child.Evaluate() switch
        {
            NodeState.Running => NodeState.Running,
            NodeState.Success => NodeState.Failure,
            NodeState.Failure => NodeState.Success,
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
