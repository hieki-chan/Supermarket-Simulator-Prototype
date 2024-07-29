using Hieki.Utils;
using Supermarket;
using Supermarket.Customers;
using Supermarket.Products;
using UnityEngine;

public class PayingState : CustomerStateBase
{
    [Viewable]
    bool paid;

    public PayingState(Customer customer) : base(customer)
    {
        transitions = new CustomerTransition[1]
        {
            new CustomerTransition(typeof(WalkingState), Paid),
        };
    }

    public override void OnStateEnter()
    {
        paid = false;
        Customer.currentStorage = null;
        CheckoutDesk checkoutDesk = SupermarketManager.Mine.CheckoutDesk;

        for (int i = 0; i < Customer.productsInBag.Count; i++)
        {
            ProductOnSale product = Customer.productsInBag[i];

            product.EnableInteracttion();
            product.transform.parent = null;
            checkoutDesk.ProductPackingPos(product);
        }

        checkoutDesk.OnPackedDone += OnPacked;
    }

    public override void OnStateExit()
    {
        CheckoutDesk checkoutDesk = SupermarketManager.Mine.CheckoutDesk;
        checkoutDesk.OnPackedDone -= OnPacked;
    }

    void OnPacked()
    {
        PaymentObject paymentObject;

        if(Customer.type == CustomerType.Poor)
        {
            paymentObject = Customer.PaymentObjectPool.Get(PaymentType.Cash, Customer.handHoldTarget.position, Customer.handHoldTarget.rotation);
        }
        else
        {
            paymentObject = Customer.PaymentObjectPool.Get((PaymentType)Random.Range(0, 2), Customer.handHoldTarget.position, Customer.handHoldTarget.rotation);
        }

        paymentObject.transform.parent = Customer.handHoldTarget;
        paymentObject.OnPayCorrected += () => { paid = true; };
        Customer.productsInBag.Clear();

        Customer.m_Animator.CrossFade(Customer.GiveHash, .02f);

        Flag.Active(1, null, () =>
        {
            paymentObject.transform.parent = null;
        });
    }

    bool Paid()
    {
        return paid;
    }
}
