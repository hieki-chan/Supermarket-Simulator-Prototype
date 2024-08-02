using Supermarket.Customers;
using Hieki.AI;
using Hieki.Utils;


public class FindAvailableStorage : Node<Customer>
{
    public FindAvailableStorage(Customer customer) : base(customer) { }

    public override NodeState Evaluate()
    {
        component.currentStorage ??= SupermarketManager.Mine.GetAvailableStorage();
        component.currentStorage ??= SupermarketManager.Mine.Storages.PickOne();

        return currentState = component.currentStorage? NodeState.Success : NodeState.Failure;
    }
}
