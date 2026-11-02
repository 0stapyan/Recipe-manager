using RecipeManager.Events;

namespace RecipeManager.logs
{
    public sealed class PublisherEventLoggerAdapter : IEventLogger
    {
        private readonly EventPublisher _publisher;

        public PublisherEventLoggerAdapter(EventPublisher publisher)
        {
            _publisher = publisher;
        }

        public void Log(string eventName, object? data = null)
        {
            _publisher.Publish(eventName, data);
        }
    }
}