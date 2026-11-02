using RecipeManager.Events;

namespace RecipeManager.logs
{
    public class FileEventSubscriber : IEventSubscriber
    {
        private readonly FileEventLogger _logger;

        public FileEventSubscriber(FileEventLogger logger)
        {
            _logger = logger;
        }

        public void OnEvent(string eventName, object? data)
        {
            _logger.Log(eventName, data);
        }
    }
}