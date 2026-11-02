using System;
using System.Globalization;
using System.Linq;
using RecipeManager.logs; 

namespace RecipeManager.main.Actions
{
    public class CookAction : IAction
    {
        public string Name => "cook";
        
        private readonly IEventLogger _logger;

        public CookAction(IEventLogger logger) 
        {
            _logger = logger;
        }
        
        public CommandResult Execute(string username, RecipeService recipeService, string recipeName, string[] parameters)
        {
            decimal multiplier = 1;

            if (parameters.Length > 1)
            {
                return CommandResult.Failure("Action 'cook' takes at most one parameter: [multiplier].");
            }
            if (parameters.Length == 1 &&
                !decimal.TryParse(parameters[0], NumberStyles.Any, CultureInfo.InvariantCulture, out multiplier))
            {
                return CommandResult.Failure("Invalid multiplier format. Must be a number.");
            }
            
            var res = recipeService.CookRecipe(recipeName, multiplier);

            if (res.IsSuccess)
            {
                _logger.Log("recipe_cooked", new { user_name = username, recipe_name = recipeName, servings = multiplier });
            }

            return res;
        }
    }
}