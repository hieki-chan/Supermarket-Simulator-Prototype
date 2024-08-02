using Hieki.AI;
using Supermarket.Customers;

public class OnPathCompleted : Node<Customer>
{
    public OnPathCompleted(Customer customer)  : base(customer) { }

    public override NodeState Evaluate()
    {
        component.currentNode = 0;
        component.OnPathComplete?.Invoke();
        return currentState = NodeState.Success;
    }
}
