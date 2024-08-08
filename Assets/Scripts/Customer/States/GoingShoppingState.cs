using UnityEngine;
using Supermarket.Customers;

public class GoingShoppingState : CustomerStateBase
{
    public GoingShoppingState() { }
    public GoingShoppingState(CustomerSM_Model SM_model) : base(SM_model) { }

    public override void OnStateEnter()
    {
        GetChooseCount();
    }

    public override void OnStateUpdate()
    {
        SM.SwitchState<HeadingToStorageState>();
    }

    private void GetChooseCount()
    {
        int takeCount = Random.Range(1, 4);

        takeCount += customer.type switch
        {
            CustomerType.Poor => Random.Range(-1, 1),
            CustomerType.MiddleClass => Random.Range(1, 2),
            CustomerType.Rich => Random.Range(1, 4),
            _ => 0
        };

        takeCount += customer.mood switch
        {
            CustomerMood.Normal => Random.Range(0, 2),
            CustomerMood.Happy => Random.Range(1, 3),
            CustomerMood.Sad => Random.Range(-1, 1),
            CustomerMood.Anger => Random.Range(-3, 0),
            _ => 0
        };

        SM.chooseCount = Mathf.Clamp(takeCount, 1, global::Supermarket.Customers.Customer.MAX_PRODUCTS);
        SM.chooseCount = 0;
    }
}
