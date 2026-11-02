using System;
using RecipeManager.logs;

namespace RecipeManager.main.Commands.SubCommands
{
    public class RecipeStepsAddSubCommand : ISubCommand
    {
        public string Name => "steps"; 
        private readonly IEventLogger _logger;

        public RecipeStepsAddSubCommand(IEventLogger logger)
        {
            _logger = logger;
        }

        public CommandResult Execute(UserService userService, string[] args)
        {
            if (userService.CurrentUser == null)
            {
                return CommandResult.Failure("Execution context error. User is not logged in.");
            }
            
            if (args.Length < 5 || args[2].ToLowerInvariant() != "add")
            {
                return CommandResult.Failure("Invalid 'recipe steps' format. Expected: recipe steps add <recipe> <step_text>");
            }

            var username = userService.CurrentUser.Username;
            var recipeService = userService.CurrentUser.RecipeService;
            
            string recipeNameStep = args[3];
            string stepText = args[4];

            var stepRes = recipeService.AddStep(recipeNameStep, stepText);
            
            if (stepRes.IsSuccess)
            {
                 _logger.Log("recipe_step_added", new { user_name = username, recipe_name = recipeNameStep });
            }
            
            return stepRes;
        }
    }
}