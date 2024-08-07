using Hieki.Pubsub;
using Supermarket;
using Supermarket.Player;

public sealed class Laptop : Interactable
{
    Topic shopTopic = Topic.FromString("shop-state");
    Topic playerCtrlTopic = Topic.FromMessage<ControlStateMessage>();
    Topic interactTopic = Topic.FromMessage<InteractMessage>();

    IPublisher publisher = new Publisher();
    ISubscriber subscriber = new Subscriber();

    bool isBuying;

    protected override void Awake()
    {
        base.Awake();
        subscriber.Subscribe<OnlineShopMessage>(shopTopic, OnShopClosed);
    }

    public override void OnInteract(PlayerController targetPlayer)
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
                publisher.Publish(playerCtrlTopic, new ControlStateMessage(true));
                publisher.Publish(interactTopic, new InteractMessage(null));
                isBuying = false;
                break;

            case true:
                publisher.Publish(playerCtrlTopic, new ControlStateMessage(false));
                publisher.Publish(interactTopic, new InteractMessage(this));
                break;
        }
    }
}
