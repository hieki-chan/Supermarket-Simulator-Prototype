using Hieki.Utils;
using Supermarket.Customers;

public class WalkingState : CustomerStateBase
{
    public WalkingState() { }
    public WalkingState(CustomerSM_Model SM_model) : base(SM_model) { }

    public override void OnStateEnter()
    {
        customer.m_Animator.DynamicPlay(Customer.WalkingHash, .02f);

        MovementPath path = customer.path;

        if (SM.currentNode == 0 || SM.currentNode == customer.path.Count - 1)
        {
            int currentNode = SM.currentNode = 0;
            SM.targetPosition = path.Nodes[currentNode].vertex;
        }
        else
        {
            SM.targetPosition = path.Nodes[SM.currentNode].vertex;
        }
    }
    public override void OnStateUpdate()
    {
        Customer customer = SM.customer;

        if (customer.Reached(SM.targetPosition))
        {
            if (Next())
                Complete();
        }
        else
        {
            Walk();
        }

        if (SM.isShopping)
        {
            SM.SwitchState<GoingShoppingState>();
        }
    }

    void Walk()
    {
        //MovementPath path = customer.path;
        //int currentNode = customer.currentNode;

        customer.MoveTowards(SM.targetPosition);

        //int previousNode = currentNode - 1;
        //if (previousNode < 0)
        //    previousNode = 0;

        //Transform.rotation = Quaternion.Slerp(Transform.rotation, Quaternion.Euler(0, path.Nodes[previousNode].rotateY, 0), Time.deltaTime * 3);
        //Customer.Look(path.Nodes[previousNode].rotateY, 7);
        //Customer.Look(targetPosition - Transform.position, 7);
    }

    bool Next()
    {
        int currNode = ++SM.currentNode;
        if (currNode >= customer.path.Count)
        {
            return true;
        }
        //next
        MovementPath path = customer.path;
        int currentNode = SM.currentNode;
        SM.targetPosition = path.Nodes[currentNode].PositionInRange();

        return false;
    }

    void Complete()
    {
        SM.currentNode = 0;
        SM.targetPosition = customer.path.Nodes[0].vertex;
        customer.OnPathComplete?.Invoke();
    }
}