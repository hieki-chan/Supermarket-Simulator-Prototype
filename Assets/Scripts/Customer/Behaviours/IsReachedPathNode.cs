using Hieki.AI;
using Supermarket.Customers;

public class IsReachedPathNode : Node<Customer>
{
    public IsReachedPathNode(Customer customer)  : base(customer) { }

    public override NodeState Evaluate()
    {
        if (component.Reached(component.targetPosition))
        {
            return currentState = NodeState.Success;
        }

        return currentState = NodeState.Failure;
    }
}
