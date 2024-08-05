using Hieki.AI;
using Supermarket.Customers;

public class IsInWaitingQueue : Node<Customer>
{
    public IsInWaitingQueue(Customer customer) : base(customer) { }

    protected override NodeState Evaluate()
    {
        component.MoveTowards(component.targetPosition);

        return currentState = NodeState.Running;
    }
}