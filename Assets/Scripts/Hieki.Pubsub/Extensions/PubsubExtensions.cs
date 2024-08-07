using UnityEngine;

namespace Hieki.Pubsub
{
    public static class PubsubExtensions
    {
        public static void Publish<T, T1>(this MonoBehaviour _, Topic  topic, T message) where T : IMessage
        {
            EventHub.Publish(topic, message);
        }

        public static S Subscribe<T, S>(this MonoBehaviour _, Topic topic, CallbackMessage<T> callback) where T : IMessage where S : ISubscriber, new()
        {
            S subscriber = new();

            EventHub.Subscribe(subscriber, topic, callback);

            return subscriber;
        }
    }
}
