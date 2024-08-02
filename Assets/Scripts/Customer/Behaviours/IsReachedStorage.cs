using Hieki.AI;
using Supermarket.Customers;
using UnityEngine;

public class IsReachedStorage : Node<Customer>
{
    public IsReachedStorage(Customer customer) : base(customer) { }

    public override NodeState Evaluate()
    {
        if (component.Reached(component.currentStorage.transform.position))
        {

            Debug.Log("a");
            return currentState = NodeState.Success;
        }


        return currentState = NodeState.Failure;
    }
}
