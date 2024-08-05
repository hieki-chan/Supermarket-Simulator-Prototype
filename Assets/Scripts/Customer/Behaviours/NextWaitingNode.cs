using UnityEngine;
using Hieki.AI;
using Hieki.Utils;
using Supermarket.Customers;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using static CheckoutDesk;

public class NextWaitingNode : Node<Customer>
{
    CheckoutPoint checkoutPoint;
    CheckoutDesk checkoutDesk;
    int index;

    public NextWaitingNode(Customer customer) : base(customer) { }

    protected override void OnPerform()
    {
        checkoutDesk = SupermarketManager.Mine.CheckoutDesk;
        (checkoutPoint, index) = checkoutDesk.GetEmptyPoint();
    }

    protected override NodeState Evaluate()
    {
        if (component.Reached(checkoutPoint.position))
        {
            Look().Forget();
            component.m_Animator.DynamicPlay(Customer.IdlingHash, .05f);

            if (index == 0)
            {
                return currentState = NodeState.Success;
            }
        }
        else
        {
            component.MoveTowards(checkoutPoint.position);
        }

        checkoutPoint.isTaked = false;
        var (_checkoutPoint, _index) = checkoutDesk.GetEmptyPoint();

        if (_index < index)
        {
            checkoutPoint = _checkoutPoint;
            index = _index;
        }
        checkoutPoint.isTaked = true;

        return currentState = NodeState.Running;
    }

    private async UniTaskVoid Look()
    {
        await component.transform.DORotate(new Vector3(0, checkoutPoint.rotateY, 0), .25f);
    }
}