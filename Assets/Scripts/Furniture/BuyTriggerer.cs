using Hieki.Utils;
using Supermarket.Customers;
using UnityEngine;

public sealed class BuyTriggerer : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent<Customer>(out var customer) && RandomUtils.Chance(.4f))
        {
            customer.StateHandler.SwitchState<HeadingToStorageState>();
        }
    }
}
