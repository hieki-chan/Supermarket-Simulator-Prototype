using Hieki.AI;
using Supermarket.Customers;
using System;

public class ChoosingProducts : Node<Customer>
{
    public ChoosingProducts(Customer customer) : base(customer) { }

    protected override NodeState Evaluate()
    {
        throw new NotImplementedException();
    }

    private void TryTake()
    {

    }

    private void ToOtherStorage()
    {

    }
}
