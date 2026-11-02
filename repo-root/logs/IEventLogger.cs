namespace RecipeManager.logs
{
    public interface IEventLogger
    {
        void Log(string eventName, object? @params = null);
    }
}