
using System;
using System.Collections.Generic;

namespace Hieki.Pubsub
{
    public class Subscriber : ISubscriber
    {
        public Dictionary<Topic, Delegate> callbackMessages { get; set; }

        public void Subscribe<T>(Topic topic, CallbackMessage<T> callbackMessage) where T : IMessage
        {
            EventHub.Subscribe(this, topic, callbackMessage);
        }
    }
}