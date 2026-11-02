namespace RecipeManager
{
    public class CommandResult
    {
        public bool IsSuccess { get; }
        public string Message { get; }

        private CommandResult(bool success, string message)
        {
            IsSuccess = success;
            Message = message;
        }

        public static CommandResult Success(string message = "Command executed successfully.")
        {
            return new CommandResult(true, message);
        }

        public static CommandResult Failure(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
                message = "An unknown error occurred.";
                
            return new CommandResult(false, message);
        }
    }
}