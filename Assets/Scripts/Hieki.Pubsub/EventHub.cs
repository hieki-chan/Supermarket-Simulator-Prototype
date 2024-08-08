using System.Collections.Generic;
using UnityEngine;

namespace Hieki.Pubsub
{
    public static class EventHub
    {
        private static Dictionary<Topic, List<ISubscriber>> m_Subscribers = new Dictionary<Topic, List<ISubscriber>>();

        public static void Subscribe<T>(ISubscriber subscriber, Topic topic, CallbackMessage<T> callbackMessage) where T : IMessage
        {
            if(subscriber.callbackMessages == null)
            {
                subscriber.callbackMessages = new Dictionary<Topic, System.Delegate> ();
            }

            if(!subscriber.callbackMessages.TryAdd(topic, callbackMessage))
            {
#if UNITY_EDITOR
                Debug.LogWarning($"Multi-Sub not supported: cannot subscribe {topic.id} more than 1 time: {subscriber}");
#endif
            }

            if (m_Subscribers.TryGetValue(topic, out List<ISubscriber> subscribers))
            {
                subscribers.Add(subscriber);
            }
            else
            {
                m_Subscribers.Add(topic, new List<ISubscriber>() { subscriber });
            }
        }

        public static void Unsubscribe(ISubscriber subscriber, Topic topic)
        {
            if (m_Subscribers.TryGetValue(topic, out List<ISubscriber> subscribers))
            {
                subscribers.Remove(subscriber);
            }
        }

        public static void Publish<T>(Topic topic, T message) where T : IMessage
        {
            if (!m_Subscribers.TryGetValue(topic, out List<ISubscriber> subscribers))
            {
                return;
            }

            foreach (var subscriber in subscribers)
            {
                if (!subscriber.callbackMessages.TryGetValue(topic, out var callback))
                {
                    continue;
                }

                (callback as CallbackMessage<T>)?.Invoke(message);
            }
        }

#if UNITY_EDITOR
        //domain reload

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void ReLoad()
        {
            m_Subscribers.Clear();
        }
#endif
    }
}
