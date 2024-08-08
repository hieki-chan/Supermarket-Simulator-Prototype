using Supermarket.Customers;
using Supermarket.Products;
using UnityEngine;
using static Hieki.Utils.RandomUtils;

public class ChoosingState : CustomerStateBase
{
    public ChoosingState() { }
    public ChoosingState(CustomerSM_Model SM_model) : base(SM_model) { }

    public override void OnStateEnter()
    {
        bool hasProduct = TryTakeProduct();

        if (ChoosingDone())
        {
            SM.SwitchState<WaitingState>();
            return;
        }

        if (hasProduct)
        {
            if(!SM.storage.IsAvailable())
            {
                if (SM.productsInBag.Count > 0)
                {
                    SM.SwitchState<WaitingState>();
                    return;
                }
            }
            if (Chance(.6f))    //move to other storage?
                return;

            //find an available other storage
            Storage storage = AvaiableStorage();

            if (!storage)
            {
                return;
            }

            SM.storage = storage;
            SM.SwitchState<HeadingToStorageState>();
            return;
        }

        //there's no more products?

        if (SM.productsInBag.Count > 0)
        {
            SM.SwitchState<WaitingState>();
            return;
        }

        if (!AvaiableStorage())
        {
            customer.Say("why there's nothing!");
            SM.storage = null;
            SM.isShopping = false;

            SM.SwitchState<WalkingState>();
        }
    }

    public override void OnStateUpdate()
    {
        SM.SwitchState<TryTakeProductState>();
    }

    bool TryTakeProduct()
    {
        Storage currentStorage = SM.storage;
        ProductOnSale product = currentStorage.TakeProduct();

        //Debug.Log(product, product);

        if (product == null)
            return false;

        product.transform.parent = customer.handHoldTarget;
        product.transform.localPosition = Vector3.zero;
        SM.productsInBag.Add(product);

        return true;
    }

    Storage AvaiableStorage()
    {
        return SupermarketManager.Mine.GetAvailableStorage();
    }

    bool ChoosingDone()
    {
        return SM.productsInBag.Count == SM.chooseCount && SM.productsInBag.Count > 0;
    }
}