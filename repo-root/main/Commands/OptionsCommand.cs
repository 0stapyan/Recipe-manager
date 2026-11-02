using System;
using System.Text;
using RecipeManager.main.Actions;

namespace RecipeManager.main.Commands
{
    public class OptionsCommand : ICommand
    {
        public string Name => "options";
        public bool RequiresLogin => true;
        
        private readonly ActionRegistry _actionRegistry;

        public OptionsCommand(ActionRegistry actionRegistry)
        {
            _actionRegistry = actionRegistry;
        }

        public CommandResult Execute(UserService userService, string[] args)
        {

            if (args.Length != 2) 
                return CommandResult.Failure("Invalid 'options' format. Expected: options <recipe_name>");
            
            string recipeName = args[1];
            
            if (userService.CurrentUser == null) 
                return CommandResult.Failure("Execution context error. User is not logged in.");

            var recipeService = userService.CurrentUser.RecipeService;
            var recipe = recipeService.GetRecipe(recipeName); 
            if (recipe == null) 
                return CommandResult.Failure($"Recipe '{recipeName}' not found.");
            
            var options = new StringBuilder("Available actions for ");
            options.AppendLine($"'{recipeName}':");
            
            foreach (var actionName in _actionRegistry.GetAvailableActionNames())
            {
                options.AppendLine($"- {actionName}");
            }
            
            return CommandResult.Success(options.ToString());
        }
    }
}