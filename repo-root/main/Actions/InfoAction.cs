using System;
using System.Linq;

namespace RecipeManager.main.Actions
{
    public class InfoAction : IAction
    {
        public string Name => "info";
        
        public InfoAction() {} 
        
        public CommandResult Execute(string username, RecipeService recipeService, string recipeName, string[] parameters)
        {
            if (parameters.Any())
            {
                return CommandResult.Failure("Action 'info' takes no parameters.");
            }
            
            return recipeService.GetRecipeInfo(recipeName);
        }
    }
}