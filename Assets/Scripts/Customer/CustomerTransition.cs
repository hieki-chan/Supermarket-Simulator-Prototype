using System;

namespace Supermarket.Customers
{
    public class CustomerTransition : ITransition<Type>
    {
        public Type To { get; set; }

        public Func<bool> Condition { get; set; }

        public CustomerTransition(Type type, Func<bool> condition)
        {
            To = type;
            Condition = condition;
        }
    }
}