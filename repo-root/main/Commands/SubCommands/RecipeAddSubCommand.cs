using RecipeManager.logs;
using System;

namespace RecipeManager.main.Commands.SubCommands
{
    public class RecipeAddSubCommand : ISubCommand
    {
        public string Name => "add";
        private readonly IEventLogger _logger;

        public RecipeAddSubCommand(IEventLogger logger)
        {
            _logger = logger;
        }

        public CommandResult Execute(UserService userService, string[] args)
        {
            if (userService.CurrentUser == null)
            {
                return CommandResult.Failure("Execution context error. User is not logged in.");
            }
            if (args.Length != 3)
            {
                return CommandResult.Failure("Invalid 'recipe add' format. Expected: recipe add <recipe_name>");
            }
            
            var username = userService.CurrentUser.Username;
            var recipeService = userService.CurrentUser.RecipeService;
            string name = args[2];

            var addRes = recipeService.AddRecipe(name);
            
            if (addRes.IsSuccess)
            {
                _logger.Log("recipe_created", new { user_name = username, recipe_name = name });
            }
            
            return addRes;
        }
    }
}