namespace RecipeManager
{
    public interface ISubCommand
    {
        string Name { get; }
        
        CommandResult Execute(UserService userService, string[] subArgs); 
    }
}