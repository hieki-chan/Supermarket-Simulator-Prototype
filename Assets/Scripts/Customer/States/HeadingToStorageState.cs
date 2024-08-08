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
            if(!SM.storage)
                return;
        }

        Transform storageTrans = SM.storage.transform;
        customer.targetPosition = storageTrans.position + storageTrans.forward * .75f + storageTrans.right * Random.Range(-.5f, .5f);
    }

    public override void OnStateUpdate()
    {
        if (!FindAvaiableStorage())
        {
            return;
        }

        if(IsReached())
        {
            SM.SwitchState<LookAtStorageState>();
            return;
        }

        customer.MoveTowards(customer.targetPosition);
    }

    bool FindAvaiableStorage()
    {
        SM.storage = SM.storage != null ? SM.storage : SupermarketManager.Mine.GetAvailableStorage();

        return SM.storage;
    }

    bool IsReached()
    {
        if (customer.Reached(customer.targetPosition))
        {
            return true;
        }
        return false;
    }
}