using Hieki.AI;
using Hieki.Utils;

namespace Supermarket.Customers
{
    public class WalkingNode : Node<Customer>
    {
        public WalkingNode(Customer customer) : base(customer) { }

        public override NodeState Evaluate()
        {
            component.m_Animator.DynamicPlay(Customer.WalkingHash, .02f);

            //MovementPath path = component.path;
            //int currentNode = component.currentNode;

            component.MoveTowards(component.targetPosition);

            //int previousNode = currentNode - 1;
            //if (previousNode < 0)
            //    previousNode = 0;

            //Transform.rotation = Quaternion.Slerp(Transform.rotation, Quaternion.Euler(0, path.Nodes[previousNode].rotateY, 0), Time.deltaTime * 3);
            //Customer.Look(path.Nodes[previousNode].rotateY, 7);
            //Customer.Look(targetPosition - Transform.position, 7);

            return currentState = NodeState.Running;
        }
    }
}
