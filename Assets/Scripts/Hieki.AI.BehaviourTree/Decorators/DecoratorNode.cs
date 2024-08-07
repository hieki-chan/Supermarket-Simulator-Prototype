namespace Hieki.AI
{
    public abstract class DecoratorNode : Node
    {
        protected Node child;

        public DecoratorNode(Node child) 
        { 
            this.child = child;

            if(child == null)
            {
                this.child = new Leaf();
            }
        }
    }
}
