using System;
using System.Globalization;
using RecipeManager.logs;

namespace RecipeManager.main.Commands.SubCommands
{
    public class RecipeAddIngredientSubCommand : ISubCommand
    {
        public string Name => "add-ingredient";
        private readonly IEventLogger _logger;

        public RecipeAddIngredientSubCommand(IEventLogger logger)
        {
            _logger = logger;
        }

        public CommandResult Execute(UserService userService, string[] args)
        {
            if (userService.CurrentUser == null)
            {
                return CommandResult.Failure("Execution context error. User is not logged in.");
            }
            if (args.Length != 6 ||
                !decimal.TryParse(args[4], NumberStyles.Any, CultureInfo.InvariantCulture, out decimal quantity))
            {
                return CommandResult.Failure("Invalid 'recipe add-ingredient' format. Expected: recipe add-ingredient <recipe> <ingredient> <quantity> <unit>");
            }

            var username = userService.CurrentUser.Username;
            var recipeService = userService.CurrentUser.RecipeService;
            
            string recipeNameIng = args[2];
            string ingredientName = args[3];
            string unit = args[5];

            var addIngRes = recipeService.AddIngredient(recipeNameIng, ingredientName, quantity, unit);
            
            if (addIngRes.IsSuccess)
            {
                _logger.Log("recipe_ingredient", new
                {
                    user_name = username,
                    recipe_name = recipeNameIng,
                    ingredient = ingredientName,
                    quantity = quantity,
                    unit = unit
                });
            }
            
            return addIngRes;
        }
    }
}