using Hieki.Utils;
using Supermarket.Customers;
using UnityEngine;

public sealed class BuyTriggerer : MonoBehaviour
{
    [SerializeField, Range(0, 1)] private float chance;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent<Customer>(out var customer) && RandomUtils.Chance(chance))
        {
            customer.StateHandler.SwitchState<HeadingToStorageState>();
        }
    }
}
