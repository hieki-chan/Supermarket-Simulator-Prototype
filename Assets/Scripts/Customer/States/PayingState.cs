using Cysharp.Threading.Tasks;
using Supermarket;
using Supermarket.Customers;
using Supermarket.Products;
using UnityEngine;

public class PayingState : CustomerStateBase
{
    bool paid;

    public PayingState() { }

    public PayingState(CustomerSM_Model SM_model) : base(SM_model) { }

    public override void OnStateEnter()
    {
        paid = false;
        SM.storage = null;
        CheckoutDesk checkoutDesk = SupermarketManager.Mine.CheckoutDesk;

        for (int i = 0; i < SM.productsInBag.Count; i++)
        {
            ProductOnSale product = SM.productsInBag[i];

            product.EnableInteracttion();
            product.transform.parent = null;
            checkoutDesk.ProductPackingPos(product);
        }

        checkoutDesk.OnPackedDone += OnPacked;
    }

    public override void OnStateUpdate()
    {
        if (Paid())
        {
            SM.SwitchState<WalkingState>();
            return;
        }
    }

    void OnPacked()
    {
        PaymentObject paymentObject;

        if (customer.type == CustomerType.Poor)
        {
            paymentObject = Customer.PaymentObjectPool.Get(PaymentType.Cash, customer.handHoldTarget.position, customer.handHoldTarget.rotation);
        }
        else
        {
            paymentObject = Customer.PaymentObjectPool.Get((PaymentType)Random.Range(0, 2), customer.handHoldTarget.position, customer.handHoldTarget.rotation);
        }

        paymentObject.transform.parent = customer.handHoldTarget;
        paymentObject.OnPayCorrected += () =>
        {
            paid = true;
            SM.storage = null;
            SM.isShopping = false;
        };
        customer.productsInBag.Clear();

        customer.m_Animator.CrossFade(Customer.GiveHash, .02f);

        GiveMoney(paymentObject).Forget();
    }

    bool Paid() => paid;

    private async UniTaskVoid GiveMoney(PaymentObject paymentObject)
    {
        await UniTask.Delay(System.TimeSpan.FromSeconds(1));
        paymentObject.transform.parent = null;
    }
}