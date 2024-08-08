using Supermarket.Customers;
using UnityEngine;

public class TryTakeProductState : CustomerStateBase
{
    float timer;

    public TryTakeProductState() { }

    public TryTakeProductState(CustomerSM_Model SM_model) : base(SM_model) { }

    public override void OnStateEnter()
    {
        timer = 0;
        customer.m_Animator.CrossFade(Customer.ChoosingHash, .02f);
    }

    public override void OnStateUpdate()
    {
        timer += Time.deltaTime;

        if (timer >= 1.2f)
        {
            SM.SwitchState<ChoosingState>();
        }
    }
}