using UnityEngine;
using Cysharp.Threading.Tasks;
using Hieki.Utils;
using Supermarket.Customers;
using DG.Tweening;
using CP = CheckoutDesk.CheckoutPoint;

public class WaitingState : CustomerStateBase
{
    int index;
    CheckoutDesk checkoutDesk;
    CP checkoutPoint;

    public WaitingState() { }
    public WaitingState(CustomerSM_Model SM_model) : base(SM_model) { }

    public override void OnStateEnter()
    {
        SM.storage = null;
        index = 1000;

        checkoutDesk = checkoutDesk != null ? checkoutDesk : SupermarketManager.Mine.CheckoutDesk;

        if(NextInQueue())
             customer.m_Animator.CrossFade(Customer.WalkingHash, .02f);
    }

    public override void OnStateUpdate()
    {
        if (checkoutPoint == null)
        {
            customer.m_Animator.CrossFade(Customer.IdlingHash, .02f);
            if (NextInQueue())
            {
                customer.m_Animator.CrossFade(Customer.WalkingHash, .02f);
            }

            return;
        }

        if (customer.Reached(checkoutPoint.position))
        {
            customer.m_Animator.DynamicPlay(Customer.IdlingHash, .05f);
            customer.Look(checkoutPoint.rotateY);

            if (index == 0) //is at first queue?
            {
                SM.SwitchState<PayingState>();
                Look().Forget();
            }
        }
        else
        {
            customer.MoveTowards(checkoutPoint.position);
        }

        NextInQueue();
    }

    bool NextInQueue()
    {
        if(checkoutPoint != null)
            checkoutPoint.isTaked = false;

        var (_checkoutPoint, _index) = checkoutDesk.GetEmptyPoint();

        if (_index < index && _index != -1)
        {
            checkoutPoint = _checkoutPoint;
            index = _index;
        }

        if (checkoutPoint != null)
            checkoutPoint.isTaked = true;

        return checkoutPoint != null;
    }

    private async UniTaskVoid Look()
    {
        await customer.transform.DORotate(new Vector3(0, checkoutPoint.rotateY, 0), .25f);
    }
}