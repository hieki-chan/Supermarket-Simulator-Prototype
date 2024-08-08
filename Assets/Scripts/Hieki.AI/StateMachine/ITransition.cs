using System;

namespace Hieki.AI.State
{
    public interface ITransition<T> /*where T :IState*/
    {
        T To { get; }

        Func<bool> Condition {  get; }
    }
}