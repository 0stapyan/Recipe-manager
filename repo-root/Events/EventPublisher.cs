using System;
using System.Collections.Generic;

namespace RecipeManager.Events
{
    public interface IEventSubscriber
    {
        void OnEvent(string eventName, object? data);
    }

    public sealed class EventPublisher
    {
        private readonly List<IEventSubscriber> _subs = new();

        public void Subscribe(IEventSubscriber subscriber) => _subs.Add(subscriber);

        public void Publish(string eventName, object? data = null)
        {
            foreach (var s in _subs)
                s.OnEvent(eventName, data);
        }
    }
}