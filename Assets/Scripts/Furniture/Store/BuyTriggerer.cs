using Hieki.Pubsub;
using Hieki.Utils;
using Supermarket.Customers;
using UnityEngine;

public sealed class BuyTriggerer : MonoBehaviour
{
    [SerializeField, Range(0, 1)] private float chance;

    Topic buyTopic = Topic.FromMessage<CustomerBuyMessage>();
    ISubscriber subscriber = new Subscriber();

    private void Awake()
    {
        subscriber.Subscribe<CustomerBuyMessage>(buyTopic, (message) =>
        {
            gameObject.SetActive(message.state);
        });
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent<Customer>(out var customer) && RandomUtils.Chance(chance))
        {
            customer.goingShopping = true;
        }
    }
}

internal readonly struct CustomerBuyMessage : IMessage
{
    public readonly bool state;
    public CustomerBuyMessage(bool state)
    {
        this.state = state;
    }
}
