namespace RecipeManager
{
    public interface ICommandParser
    {
        CommandResult ExecuteCommand(string input);
    }
}