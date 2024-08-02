using Hieki.AI;
using Supermarket.Customers;

public class IsGoingShopping : Node<Customer>
{
    public IsGoingShopping(Customer customer) : base(customer) { }

    public override NodeState Evaluate()
    {
        return currentState = component.goingShopping ? NodeState.Success : NodeState.Failure;
    }
}
