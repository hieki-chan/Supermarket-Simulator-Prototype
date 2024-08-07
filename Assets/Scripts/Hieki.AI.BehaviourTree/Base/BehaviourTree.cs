namespace Hieki.AI
{
    public abstract class BehaviourTree /*: UnityEngine.ScriptableObject*/
    {
        protected Root root;

        public BehaviourTree()
        {

        }

        public BehaviourTree(Root root)
        {
            this.root = root;
        }

        /*protected virtual void Start()
        {
            InitTree();
        }*/
        public virtual void Process()
        {
            root?.Process();
        }
        public virtual void InitTree(Root root) => this.root = root;
    }
}
