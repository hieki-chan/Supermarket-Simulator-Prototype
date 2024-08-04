using Hieki.AI;
using Supermarket.Customers;

public class IsChoosingProducts : Node<Customer>
{
    public IsChoosingProducts(Customer customer) : base(customer) { }

    public override NodeState Evaluate()
    {
        return currentState = NodeState.Failure;
    }
}
