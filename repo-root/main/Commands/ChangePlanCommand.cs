using System;
using System.Linq;
using RecipeManager.logs; 

namespace RecipeManager.main.Commands
{
    public class ChangePlanCommand : ICommand
    {
        public string Name => "change_plan";
        public bool RequiresLogin => true;

        private readonly ILimitsService _limitsService;
        private readonly IEventLogger _logger;
        public ChangePlanCommand(ILimitsService limitsService, IEventLogger logger)
        {
            _limitsService = limitsService;
            _logger = logger;
        }

        public CommandResult Execute(UserService userService, string[] args)
        {
            if (userService.CurrentUser == null)
            {
                return CommandResult.Failure("Execution context error. User is not logged in.");
            }

            if (args.Length != 2)
            {
                return CommandResult.Failure("Invalid 'change_plan' format. Expected: change_plan <plan_name>");
            }

            string planName = args[1].Trim();
            UserProfile profile = userService.CurrentUser;
            PlanType oldPlanType = profile.PlanType;
            var username = profile.Username;

            if (!Enum.TryParse(planName, true, out PlanType newPlanType))
            {
                return CommandResult.Failure($"Unknown plan name: '{planName}'. Available: Basic, Chef.");
            }

            if (newPlanType == oldPlanType)
            {
                return CommandResult.Failure($"You are already on the {oldPlanType} plan.");
            }
            
            if (newPlanType < oldPlanType)
            {
                PlanLimits newLimits = _limitsService.GetLimits(newPlanType);

                if (profile.PantryService.GetTotalItemCount() > newLimits.MaxPantryItems)
                {
                    _logger.Log("limit_reached", new { user_name = username, limit_type = "pantry_items" });
                    return CommandResult.Failure(
                        $"Cannot downgrade to {newPlanType} plan. Current pantry item count ({profile.PantryService.GetTotalItemCount()}) exceeds the limit of {newLimits.MaxPantryItems}."
                    );
                }

                if (profile.RecipeService.GetTotalRecipeCount() > newLimits.MaxRecipes)
                {
                    _logger.Log("limit_reached", new { user_name = username, limit_type = "recipes" });
                    return CommandResult.Failure(
                        $"Cannot downgrade to {newPlanType} plan. Current recipe count ({profile.RecipeService.GetTotalRecipeCount()}) exceeds the limit of {newLimits.MaxRecipes}."
                    );
                }

                if (profile.PlanService.GetTotalPlanEntryCount() > newLimits.MaxPlanEntries)
                {
                    _logger.Log("limit_reached", new { user_name = username, limit_type = "plans" });
                    return CommandResult.Failure(
                        $"Cannot downgrade to {newPlanType} plan. Current plan entries count ({profile.PlanService.GetTotalPlanEntryCount()}) exceeds the limit of {newLimits.MaxPlanEntries}."
                    );
                }
            }

            profile.PlanType = newPlanType;
            var success = CommandResult.Success($"Plan successfully changed from {oldPlanType} to {newPlanType}. Welcome to the {newPlanType} plan!");
            _logger.Log("plan_changed", new { user_name = username, plan_name = newPlanType.ToString() });

            return success;
        }
    }
}
