using Hieki.AI;
using Supermarket.Customers;
using TMPro;

public class NextPathNode : Node<Customer>
{
    public NextPathNode(Customer customer) : base(customer) { }

    public override NodeState Evaluate()
    {
        int currNode = ++component.currentNode;
        if (currNode >= component.path.Count)
        {
            return currentState =  NodeState.Success;
        }
        //next
        MovementPath path = component.path;
        int currentNode = component.currentNode;
        component.targetPosition = path.Nodes[currentNode].PositionInRange();

        return currentState = NodeState.Failure;
    }
}
