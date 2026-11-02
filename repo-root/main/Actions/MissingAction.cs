using System;
using System.Linq;

namespace RecipeManager.main.Actions
{
    public class MissingAction : IAction
    {
        public string Name => "missing";
        
        public MissingAction() {}
        
        public CommandResult Execute(string username, RecipeService recipeService, string recipeName, string[] parameters)
        {
            if (parameters.Any())
            {
                return CommandResult.Failure("Action 'missing' takes no parameters.");
            }
            
            return recipeService.CheckMissingIngredients(recipeName);
        }
    }
}