using Hieki.AI;
using Supermarket.Customers;

public class HeadingToStorage : Node<Customer>
{
    public HeadingToStorage(Customer customer) : base(customer) { }

    public override NodeState Evaluate()
    {
        component.MoveTowards(component.currentStorage.transform.position);
        return Running();
    }
}
