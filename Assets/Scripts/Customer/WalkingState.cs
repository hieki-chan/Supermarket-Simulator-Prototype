using Supermarket.Customers;
using UnityEngine;

public class WalkingState : CustomerStateBase
{
    [Viewable]
    Vector3 targetPosition;

    public WalkingState(Customer customer) : base(customer)
    {

    }

    public override void OnStateEnter()
    {
        Customer.m_Animator.CrossFade(Customer.WalkingHash, .02f);
        targetPosition = Customer.transform.position;
    }

    public override void OnStateUpdate()
    {
        MoveAlongPath();
    }

    private void MoveAlongPath()
    {
        MovementPath path = Customer.path;
        int currentNode = Customer.currentNode;

        Customer.MoveTowards(targetPosition);

        int previousNode = currentNode - 1;
        if (previousNode < 0)
            previousNode = 0;

        //Transform.rotation = Quaternion.Slerp(Transform.rotation, Quaternion.Euler(0, path.Nodes[previousNode].rotateY, 0), Time.deltaTime * 3);
        //Customer.Look(path.Nodes[previousNode].rotateY, 7);
        //Customer.Look(targetPosition - Transform.position, 7);

        if (Customer.Reached(targetPosition))
        {
            int currNode = ++Customer.currentNode;
            if (currNode >= path.Count)
            {
                Customer.currentNode = 0;
                Customer.OnPathComplete?.Invoke();
            }

            GetTargetPosition();
        }
    }

    void GetTargetPosition()
    {
        MovementPath path = Customer.path;
        int currentNode = Customer.currentNode;
        targetPosition = path.Nodes[currentNode].PositionInRange();
    }
}
