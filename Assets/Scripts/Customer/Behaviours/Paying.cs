using UnityEngine;
using Hieki.AI;
using Supermarket;
using Supermarket.Customers;
using Supermarket.Products;
using Cysharp.Threading.Tasks;

public class Paying : Node<Customer>
{
    bool paid;

    public Paying(Customer customer) : base(customer) { }

    protected override void OnPerform()
    {
        paid = false;
        component.currentStorage = null;
        CheckoutDesk checkoutDesk = SupermarketManager.Mine.CheckoutDesk;

        for (int i = 0; i < component.productsInBag.Count; i++)
        {
            ProductOnSale product = component.productsInBag[i];

            product.EnableInteracttion();
            product.transform.parent = null;
            checkoutDesk.ProductPackingPos(product);
        }

        checkoutDesk.OnPackedDone += OnPacked;
    }

    protected override NodeState Evaluate()
    {
        if(!paid)
            return currentState = NodeState.Running;

        Release();
        return currentState = NodeState.Success;
    }

    private void OnPacked()
    {
        PaymentObject paymentObject;

        if (component.type == CustomerType.Poor)
        {
            paymentObject = Customer.PaymentObjectPool.Get(PaymentType.Cash, component.handHoldTarget.position, component.handHoldTarget.rotation);
        }
        else
        {
            paymentObject = Customer.PaymentObjectPool.Get((PaymentType)Random.Range(0, 2), component.handHoldTarget.position, component.handHoldTarget.rotation);
        }

        paymentObject.transform.parent = component.handHoldTarget;
        paymentObject.OnPayCorrected += () => { paid = true; };
        component.productsInBag.Clear();

        component.m_Animator.CrossFade(Customer.GiveHash, .02f);

        GiveMoney(paymentObject).Forget();
    }

    private async UniTaskVoid GiveMoney(PaymentObject paymentObject)
    {
        await UniTask.Delay(System.TimeSpan.FromSeconds(1));
        paymentObject.transform.parent = null;
    }
}