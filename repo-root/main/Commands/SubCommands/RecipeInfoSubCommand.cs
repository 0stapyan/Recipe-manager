using System;
using System.Linq;

namespace RecipeManager.main.Commands.SubCommands
{
    public class RecipeInfoSubCommand : ISubCommand
    {
        public string Name => "info";

        public CommandResult Execute(UserService userService, string[] args)
        {
            if (userService.CurrentUser == null)
            {
                return CommandResult.Failure("Execution context error. User is not logged in.");
            }
            if (args.Length != 3)
            {
                return CommandResult.Failure("Invalid 'recipe info' format. Expected: recipe info <recipe_name>");
            }
            
            var recipeService = userService.CurrentUser.RecipeService;
            string nameInfo = args[2];
            
            return recipeService.GetRecipeInfo(nameInfo);
        }
    }
}