using System;

namespace Supermarket.Customers
{
    public class CustomerTransition
    {
        public CustomerTree To { get; set; }

        public Func<bool> Condition { get; set; }

        public CustomerTransition(CustomerTree next, Func<bool> condition)
        {
            To = next;
            Condition = condition;
        }


        public CustomerTree Next()
        {
            return To;
        }
    }
}