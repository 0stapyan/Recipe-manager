using System;
using System.Globalization;
using RecipeManager.logs;

namespace RecipeManager.main.Commands.SubCommands
{
    public class PlanAddSubCommand : ISubCommand
    {
        public string Name => "add";
        private readonly IEventLogger _logger;

        public PlanAddSubCommand(IEventLogger logger)
        {
            _logger = logger;
        }

        public CommandResult Execute(UserService userService, string[] args)
        {
            if (userService.CurrentUser == null)
            {
                return CommandResult.Failure("Execution context error. User is not logged in.");
            }
            if (args.Length < 4 || args.Length > 5 || 
                !DateTime.TryParse(args[3], out DateTime date))
            {
                return CommandResult.Failure("Invalid 'plan add' format. Expected: plan add <recipe_name> <date> [multiplier]");
            }
            
            var username = userService.CurrentUser.Username;
            var planService = userService.CurrentUser.PlanService;
            
            string recipeName = args[2];
            decimal multiplier = 1;

            if (args.Length == 5)
            {
                if (!decimal.TryParse(args[4], NumberStyles.Any, CultureInfo.InvariantCulture, out multiplier))
                {
                    return CommandResult.Failure("Invalid multiplier format. Must be a number.");
                }
            }
            
            var addRes = planService.AddPlan(recipeName, date, multiplier);

            if (addRes.IsSuccess)
            {
                _logger.Log("planned_added", new
                {
                    user_name = username,
                    recipe_name = recipeName,
                    date = date.ToString("yyyy-MM-dd"),
                    servings = multiplier
                });
            }
            else
            {
                if (!string.IsNullOrEmpty(addRes.Message) &&
                    addRes.Message.IndexOf("limit", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    _logger.Log("limit_reached", new { user_name = username, limit_type = "plans" });
                }
            }
            return addRes;
        }
    }
}