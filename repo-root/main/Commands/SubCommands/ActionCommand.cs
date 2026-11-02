using System;
using System.Linq;
using RecipeManager.main.Actions;

namespace RecipeManager.main.Commands
{
    public class ActionCommand : ICommand
    {
        public string Name => "action";
        public bool RequiresLogin => true;
        
        private readonly ActionRegistry _actionRegistry;

        public ActionCommand(ActionRegistry actionRegistry)
        {
            _actionRegistry = actionRegistry;
        }

        public CommandResult Execute(UserService userService, string[] args)
        {

            if (args.Length < 3) 
                return CommandResult.Failure("Invalid 'action' format. Expected: action <action_name> <recipe_name> [<parameters>]");

            var actionName = args[1];
            var recipeName = args[2];
            var actionParams = args.Skip(3).ToArray();

            var action = _actionRegistry.GetAction(actionName);
            if (action == null) 
                return CommandResult.Failure($"Action '{actionName}' is not recognized.");
            
            if (userService.CurrentUser == null)
                return CommandResult.Failure("Execution context error. User is not logged in.");
                
            var userProfile = userService.CurrentUser;
            var recipeService = userProfile.RecipeService;
            var username = userProfile.Username;

            if (recipeService.GetRecipe(recipeName) == null) 
                return CommandResult.Failure($"Recipe '{recipeName}' not found.");
            
            return action.Execute(username, recipeService, recipeName, actionParams);
        }
    }
}