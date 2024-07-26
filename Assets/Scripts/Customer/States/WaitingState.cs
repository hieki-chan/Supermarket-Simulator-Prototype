using Supermarket.Customers;
using Hieki.Utils;
using CheckoutPoint = CheckoutDesk.CheckoutPoint;

public class WaitingState : CustomerStateBase
{
    CheckoutDesk checkoutDesk;
    CheckoutPoint checkoutPoint;
    int index;
    bool pay;

    public WaitingState(Customer customer) : base(customer)
    {
        checkoutDesk = SupermarketManager.Mine.CheckoutDesk;

        transitions = new CustomerTransition[1]
        {
            new CustomerTransition(typeof(PayingState), Pay)
        };
    }

    public override void OnStateEnter()
    {
        pay = false;
        (checkoutPoint, index) = checkoutDesk.GetEmptyPoint();
        if(checkoutPoint == null)
        {
            return;
        }
        checkoutPoint.isTaked = true;

        Customer.m_Animator.CrossFade(Customer.WalkingHash, .02f);
    }

    public override void OnStateUpdate()
    {
        Customer.MoveTowards(checkoutPoint.position);

        if (Customer.Reached(checkoutPoint.position))
        {
            Customer.transform.LeanRotateY(checkoutPoint.rotateY, .25f);
            Customer.m_Animator.DynamicPlay(Customer.IdlingHash, .05f);

            if(index == 0)
            {
                pay = true;
            }
        }
        else
        {
            Customer.Look((checkoutPoint.position - Transform.position), 7);
        }
        checkoutPoint.isTaked = false;
        var (_checkoutPoint, _index) = checkoutDesk.GetEmptyPoint();

        if(_index < index)
        {
            checkoutPoint = _checkoutPoint;
            index = _index;
        }
        checkoutPoint.isTaked = true;
    }

    bool Pay()
    {
        return pay;
    }
}
