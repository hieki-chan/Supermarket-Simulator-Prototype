using System;
using System.Collections.Generic;

namespace Hieki.Pubsub
{
    public interface ISubscriber
    {
        Dictionary<Topic, Delegate> callbackMessages { get; set; }

        void Subscribe<T>(Topic topic, CallbackMessage<T> callbackMessage) where T : IMessage;
    }
}
