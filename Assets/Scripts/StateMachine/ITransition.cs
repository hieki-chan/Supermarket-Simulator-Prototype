using System;

namespace Supermarket
{
    public interface ITransition<T> /*where T :IState*/
    {
        T To { get; }

        Func<bool> Condition {  get; }
    }
}