using Hieki.Utils;
using Supermarket.Customers;
using Supermarket.Products;
using System.Collections;
using UnityEngine;

public class ChoosingState : CustomerStateBase
{
    Storage currentStorage;

    [Viewable]
    int takeCount;
    [Viewable]
    int takedCount;
    [Viewable]
    bool taken;
    [Viewable]
    bool noProducts;
    [Viewable]
    bool getout;

    public ChoosingState(Customer customer) : base(customer)
    {
        transitions = new CustomerTransition[2]
        {
            new CustomerTransition(typeof(WaitingState), ChooseProductsDone),
            new CustomerTransition(typeof(WalkingState), GetOut),
        };
    }

    public override void OnStateEnter()
    {
        currentStorage = Customer.currentStorage;
        takeCount = Random.Range(1, 4);

        takeCount += Customer.type switch
        {
            CustomerType.Poor => Random.Range(-1, 1),
            CustomerType.MiddleClass => Random.Range(1, 2),
            CustomerType.Rich => Random.Range(1, 4),
            _ => 0
        };

        takeCount += Customer.mood switch
        {
            CustomerMood.Normal => Random.Range(0, 2),
            CustomerMood.Happy => Random.Range(1, 3),
            CustomerMood.Sad => Random.Range(-1, 1),
            CustomerMood.Anger => Random.Range(-3, 0),
            _ => 0
        };

        takeCount = Mathf.Clamp(takeCount, 1, global::Supermarket.Customers.Customer.MAX_PRODUCTS);
        takedCount = 0;

        Customer.m_Animator.CrossFade(Customer.ChoosingHash, .02f);
        Customer.m_Animator.SetFloat(Customer.ChoosingSpeedHash, 2);

        taken = false;
        getout = false;
        noProducts = false;

        Customer.StartCoroutine(TakeProduct());
    }

    public override void OnStateUpdate()
    {
        Transform.forward = Vector3.Lerp(Transform.forward, -currentStorage.transform.forward, Time.deltaTime * 6f);
    }

    WaitForSeconds waitForTake = new WaitForSeconds(1.2f);
    WaitForSeconds wairForAnimation = new WaitForSeconds(1.38333333333F);

    IEnumerator TakeProduct()
    {
        ProductOnSale product = currentStorage.CheckProduct();
        Storage storage = SupermarketManager.Mine.GetAvailableStorage();

        if (storage)
        {
            currentStorage = Customer.currentStorage = storage;
        }

        while (takedCount < takeCount)
        {
            product = currentStorage.CheckProduct();
            Customer.m_Animator.CrossFade(Customer.ChoosingHash, .02f);
            taken = false;

            switch (Customer.type)
            {
                case CustomerType.Poor:
                    break;
                case CustomerType.MiddleClass:
                    break;
                case CustomerType.Rich:
                    break;
                default:
                    break;
            }

            yield return waitForTake;
            OnTakeProduct();

            yield return wairForAnimation;

            taken = true;
        }
    }

    public void OnTakeProduct()
    {
        ProductOnSale product = currentStorage.TakeProduct();
        if (product == null || RandomUtils.Chance(.3f))
        {
            //find an available other storage
            Storage storage = SupermarketManager.Mine.GetAvailableStorage();

            if (storage)
            {
                currentStorage = Customer.currentStorage = storage;
            }
            else if(!product) 
            {
                //there's no any products
                if (Customer.productsInBag.Count == 0)
                {
                    Customer.Say("why there's nothing!");
                    getout = true;
                }
                else
                {
                    noProducts = true;
                }
            }

            return;
        }

        product.transform.parent = Customer.handHoldTarget;
        product.transform.localPosition = Vector3.zero;
        Customer.productsInBag.Add(product);
        takedCount++;
    }

    bool ChooseProductsDone()
    {
        return takedCount == takeCount && taken || noProducts;
    }

    bool GetOut()
    {
        return getout;
    }
}
