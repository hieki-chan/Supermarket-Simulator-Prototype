namespace Hieki.Pubsub
{
    public class Publisher : IPublisher
    {
        public void Publish<T>(Topic topic, T message) where T : IMessage
        {
            EventHub.Publish(topic, message);
        }
    }
}