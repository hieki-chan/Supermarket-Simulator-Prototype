using Hieki.AI;
using Supermarket.Customers;

public class IsReachedStorage : Node<Customer>
{
    public IsReachedStorage(Customer customer) : base(customer) { }

    protected override NodeState Evaluate()
    {
        if (component.Reached(component.targetPosition))
        {
            return currentState = NodeState.Success;
        }
        return currentState = NodeState.Failure;
    }
}
