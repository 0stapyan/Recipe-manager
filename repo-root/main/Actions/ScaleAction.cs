using System;
using System.Globalization;
using System.Linq;

namespace RecipeManager.main.Actions
{
    public class ScaleAction : IAction
    {
        public string Name => "scale";

        public ScaleAction() {}
        
        public CommandResult Execute(string username, RecipeService recipeService, string recipeName, string[] parameters)
        {
            if (parameters.Length != 1)
            {
                return CommandResult.Failure("Action 'scale' requires exactly one parameter: <multiplier>.");
            }
            
            if (!decimal.TryParse(parameters[0], NumberStyles.Any, CultureInfo.InvariantCulture, out decimal multiplier))
            {
                return CommandResult.Failure("Invalid multiplier format. Must be a number.");
            }
            
            return recipeService.ScaleRecipe(recipeName, multiplier);
        }
    }
}