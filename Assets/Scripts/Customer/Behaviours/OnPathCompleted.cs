using Hieki.AI;
using Supermarket.Customers;

public class OnPathCompleted : Node<Customer>
{
    public OnPathCompleted(Customer customer)  : base(customer) { }

    protected override NodeState Evaluate()
    {
        component.currentNode = 0;
        component.targetPosition = component.path.Nodes[0].vertex;
        component.OnPathComplete?.Invoke();
        return currentState = NodeState.Success;
    }
}
