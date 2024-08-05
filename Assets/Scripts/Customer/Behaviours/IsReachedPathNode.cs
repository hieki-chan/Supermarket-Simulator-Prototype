using Hieki.AI;
using Supermarket.Customers;

public class IsReachedPathNode : Node<Customer>
{
    public IsReachedPathNode(Customer customer)  : base(customer) { }

    protected override void OnPerform()
    {
        MovementPath path = component.path;
        int currentNode = component.currentNode = 0;
        component.targetPosition = path.Nodes[currentNode].vertex;
    }

    protected override NodeState Evaluate()
    {
        if (component.Reached(component.targetPosition))
        {
            return currentState = NodeState.Success;
        }

        return currentState = NodeState.Failure;
    }
}
