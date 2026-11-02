using System;
using System.Linq;

namespace RecipeManager.main.Commands.SubCommands
{
    public class PlanListSubCommand : ISubCommand
    {
        public string Name => "list";

        public CommandResult Execute(UserService userService, string[] args)
        {
            if (userService.CurrentUser == null)
            {
                return CommandResult.Failure("Execution context error. User is not logged in.");
            }
            if (args.Length != 2)
            {
                return CommandResult.Failure("Invalid 'plan list' format. Expected: plan list");
            }
            
            var planService = userService.CurrentUser.PlanService;
            
            return planService.GetPlanList();
        }
    }
}