namespace Hieki.AI
{
    /// <summary>
    /// Node with a component value that inherits from <see cref="ITreeComponent"/>.
    /// </summary>
    /// <typeparam name="T">component</typeparam>
    public abstract class Node<T> : Node where T : ITreeComponent
    {
        protected T component;
        /// <summary>
        /// current state of the node.
        /// </summary>
        public Node(T component) { this.component = component; }

        //public static implicit operator bool(Node<T> node)
        //{
        //    return node is not null;
        //}
    }
}
