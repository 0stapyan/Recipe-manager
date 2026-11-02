using System.Linq;

namespace RecipeManager
{
    public interface ICommand
    {
        string Name { get; }
        bool RequiresLogin { get; }
        
        CommandResult Execute(UserService userService, string[] args);
    }
}