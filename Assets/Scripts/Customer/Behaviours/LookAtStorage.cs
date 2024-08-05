using Hieki.AI;
using Supermarket.Customers;
using Hieki.Utils;

public class LookAtStorage : Node<Customer>
{
    public LookAtStorage(Customer customer) : base(customer) { }

    protected override NodeState Evaluate()
    {
        component.m_Animator.DynamicPlay(Customer.IdlingHash, .02f);
        component.Look(-component.currentStorage.transform.forward);

        return currentState = NodeState.Success;
    }
}