using Hieki.Utils;
using Supermarket.Customers;

public class WalkingState : CustomerStateBase
{
    public WalkingState() { }
    public WalkingState(CustomerSM_Model SM_model) : base(SM_model) { }

    public override void OnStateEnter()
    {
        customer.m_Animator.DynamicPlay(Customer.WalkingHash, .02f);

        if(customer.currentNode == 0 || customer.currentNode == customer.path.Count - 1)
        {
            MovementPath path = customer.path;
            int currentNode = customer.currentNode = 0;
            customer.targetPosition = path.Nodes[currentNode].vertex;
        }
    }
    public override void OnStateUpdate()
    {
        Customer customer = SM.customer;

        if (customer.Reached(customer.targetPosition))
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

        customer.MoveTowards(customer.targetPosition);

        //int previousNode = currentNode - 1;
        //if (previousNode < 0)
        //    previousNode = 0;

        //Transform.rotation = Quaternion.Slerp(Transform.rotation, Quaternion.Euler(0, path.Nodes[previousNode].rotateY, 0), Time.deltaTime * 3);
        //Customer.Look(path.Nodes[previousNode].rotateY, 7);
        //Customer.Look(targetPosition - Transform.position, 7);
    }

    bool Next()
    {
        int currNode = ++customer.currentNode;
        if (currNode >= customer.path.Count)
        {
            return true;
        }
        //next
        MovementPath path = customer.path;
        int currentNode = customer.currentNode;
        customer.targetPosition = path.Nodes[currentNode].PositionInRange();

        return false;
    }

    void Complete()
    {
        customer.currentNode = 0;
        customer.targetPosition = customer.path.Nodes[0].vertex;
        customer.OnPathComplete?.Invoke();
    }
}