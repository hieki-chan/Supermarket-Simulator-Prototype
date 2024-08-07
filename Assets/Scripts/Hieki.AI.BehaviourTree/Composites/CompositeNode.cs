using System.Collections.Generic;

namespace Hieki.AI
{
    public abstract class CompositeNode : Node
    {
        protected readonly List<Node> children;
        /// <summary>
        /// current state of the node.
        /// </summary>
        public CompositeNode() { this.children = new List<Node>(); }

        public CompositeNode(List<Node> children)
        {
            this.children = children;
        }
    }
}
