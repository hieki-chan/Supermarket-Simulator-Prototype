using Hieki.AI;
using Supermarket.Customers;

public class IsChoosingProducts : Node<Customer>
{
    public IsChoosingProducts(Customer customer) : base(customer) { }

    protected override NodeState Evaluate()
    {
        return currentState = NodeState.Failure;
    }
}
