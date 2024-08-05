using UnityEngine;
using Hieki.AI;
using Supermarket.Customers;

public class HeadingToStorage : Node<Customer>
{
    public HeadingToStorage(Customer customer) : base(customer) { }

    protected override void OnPerform()
    {
        Transform storageTrans = component.currentStorage.transform;
        component.targetPosition = storageTrans.position + storageTrans.forward * .75f + storageTrans.right * Random.Range(-.5f, .5f);
    }

    protected override NodeState Evaluate()
    {
        component.MoveTowards(component.targetPosition);
        return Running();
    }
}
