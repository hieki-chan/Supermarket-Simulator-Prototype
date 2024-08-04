using Hieki.AI;
using Supermarket.Customers;
using System;

public class ChoosingProducts : Node<Customer>
{
    public ChoosingProducts(Customer customer) : base(customer) { }

    public override NodeState Evaluate()
    {
        throw new NotImplementedException();
    }
}
