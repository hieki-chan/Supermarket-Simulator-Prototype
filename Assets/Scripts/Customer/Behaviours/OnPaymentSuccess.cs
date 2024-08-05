using Hieki.AI;
using Supermarket.Customers;

public class OnPaymentSuccess : Node<Customer>
{
    public OnPaymentSuccess(Customer customer) : base(customer) { }

    protected override NodeState Evaluate()
    {
        component.currentStorage = null;
        component.goingShopping = false;
        return currentState = NodeState.Success;
    }
}