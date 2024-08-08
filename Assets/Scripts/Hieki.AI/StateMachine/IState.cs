namespace Hieki.AI.State
{
    public interface IState
    {
        void OnStateEnter();

        void OnStateUpdate();

        void OnStateExit();

        //public T IsState<T>() where T : IState
        //{
        //    return (T)this;
        //}

        //public bool IsState<T>(out T state) where T : IState
        //{
        //    state = (T)this;
        //    return state != null;
        //}

        //        public void print(object message, Object context)
        //        {
        //#if UNITY_EDITOR
        //            Debug.Log(message, context);
        //#endif
        //        }
    }

    public interface IState<T> : IState where T : class //class should be a state machine
    {
        T SM { get; set; }
    }
}