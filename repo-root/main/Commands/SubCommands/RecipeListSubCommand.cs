using System;
using System.Linq;

namespace RecipeManager.main.Commands.SubCommands
{
    public class RecipeListSubCommand : ISubCommand
    {
        public string Name => "list";

        public CommandResult Execute(UserService userService, string[] args)
        {
            if (userService.CurrentUser == null)
            {
                return CommandResult.Failure("Execution context error. User is not logged in.");
            }
            if (args.Length != 2)
            {
                return CommandResult.Failure("Invalid 'recipe list' format. Expected: recipe list");
            }

            var recipeService = userService.CurrentUser.RecipeService;
            
            var recipes = recipeService.GetRecipes();
            if (!recipes.Any())
            {
                return CommandResult.Success("No recipes found.");
            }

            var message = "Available Recipes:\n" + string.Join("\n", recipes.Select(r => $"- {r.Name}"));
            return CommandResult.Success(message);
        }
    }
}