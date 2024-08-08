using Hieki.Utils;
using Supermarket.Customers;
using UnityEngine;

public class LookAtStorageState : CustomerStateBase
{
    public LookAtStorageState() { }
    public LookAtStorageState(CustomerSM_Model SM_model) : base(SM_model) { }

    public override void OnStateEnter()
    {
        customer.m_Animator.DynamicPlay(Customer.IdlingHash, .02f);
    }

    public override void OnStateUpdate()
    {
        Vector3 dir = -SM.storage.transform.forward;
        customer.Look(dir);

        if (Vector3.Dot(customer.transform.forward, dir) >= .8f)
        {
            //success    

            SM.SwitchState<TryTakeProductState>();
        }

        //running
    }
}