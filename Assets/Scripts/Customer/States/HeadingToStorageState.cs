using Hieki.Utils;
using Supermarket.Customers;
using UnityEngine;

public class HeadingToStorageState : CustomerStateBase
{
    public HeadingToStorageState() { }
    public HeadingToStorageState(CustomerSM_Model SM_model) : base(SM_model) { }

    public override void OnStateEnter()
    {
        customer.m_Animator.SetFloat(Customer.ChoosingSpeedHash, 2);
        customer.m_Animator.DynamicPlay(Customer.WalkingHash, .02f);

        if (!FindAvaiableStorage())
        {
            SM.storage = SM.storage != null ? SM.storage : SupermarketManager.Mine.Storages.PickOne();
            if (!SM.storage)
                return;
        }

        Transform storageTrans = SM.storage.transform;
        SM.targetPosition = storageTrans.position + storageTrans.forward * .75f + storageTrans.right * Random.Range(-.5f, .5f);
    }

    public override void OnStateUpdate()
    {
        if (!FindAvaiableStorage())
        {
            return;
        }

        if (IsReached())
        {
            SM.SwitchState<LookAtStorageState>();
            return;
        }

        customer.MoveTowards(SM.targetPosition);
    }

    bool FindAvaiableStorage()
    {
        Storage storage;
        if (SM.storage == null || !SM.storage.IsAvailable())
        {
            storage = SupermarketManager.Mine.GetAvailableStorage();
            if(storage != null)
            {
                if(SM.storage != storage)
                {
                    Transform storageTrans = storage.transform;
                    SM.targetPosition = storageTrans.position + storageTrans.forward * .75f + storageTrans.right * Random.Range(-.5f, .5f);
                }
                SM.storage = storage;
            }
        }

        return SM.storage;
    }

    bool IsReached()
    {
        if (customer.Reached(SM.targetPosition))
        {
            return true;
        }
        return false;
    }
}