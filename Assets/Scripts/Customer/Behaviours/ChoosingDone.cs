using Hieki.AI;
using Supermarket.Customers;

public class ChoosingDone : Node<Customer>
{
    public ChoosingDone(Customer customer) : base(customer) { }

    protected override NodeState Evaluate()
    {
        return currentState = NodeState.Failure;
    }
}
