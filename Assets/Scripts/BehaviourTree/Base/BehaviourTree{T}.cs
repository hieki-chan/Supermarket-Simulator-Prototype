namespace Hieki.AI
{
    public abstract class BehaviourTree<T> : BehaviourTree where T : ITreeComponent
    {
        public abstract void InitTree(T component);
    }
}
