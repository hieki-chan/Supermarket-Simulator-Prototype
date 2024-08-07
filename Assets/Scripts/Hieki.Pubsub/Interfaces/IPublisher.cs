using System;

namespace Hieki.Pubsub
{
    public interface IPublisher
    {
        public void Publish<T>(Topic topic, T message) where T : IMessage;
    }
}
