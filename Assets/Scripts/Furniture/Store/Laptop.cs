using Hieki.Pubsub;
using Supermarket;
using Supermarket.Player;
using UnityEngine;

public sealed class Laptop : Interactable
{
    Topic shopTopic = Topic.FromString("shop-state");

    IPublisher publisher = new Publisher();
    ISubscriber subscriber = new Subscriber();

    bool isBuying;

    protected override void Awake()
    {
        base.Awake();
        subscriber.Subscribe<OnlineShopMessage>(shopTopic, OnShopClosed);
    }

    public override void OnInteract(Transform playerTrans, Transform cameraTrans)
    {
        isBuying = true;

        publisher.Publish(shopTopic, new OnlineShopMessage(true));
    }

    public void OnShopClosed(OnlineShopMessage message)
    {
        if (isBuying == false)
            return;

        switch (message.state)
        {
            case false:
                publisher.Publish(PlayerTopics.controlTopic, new ControlStateMessage(true));
                publisher.Publish(PlayerTopics.interactTopic, new InteractMessage(null));
                isBuying = false;
                break;

            case true:
                publisher.Publish(PlayerTopics.controlTopic, new ControlStateMessage(false));
                publisher.Publish(PlayerTopics.interactTopic, new InteractMessage(this));
                break;
        }
    }
}
