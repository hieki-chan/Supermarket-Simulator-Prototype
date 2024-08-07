using Hieki.Pubsub;

public readonly struct OnlineShopMessage : IMessage
{
    public readonly bool state;

    public OnlineShopMessage(bool state)
    {
        this.state = state;
    }
}