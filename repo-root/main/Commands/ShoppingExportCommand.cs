using System;
using System.Linq;
using RecipeManager.logs;

namespace RecipeManager.main.Commands
{
    public class ShoppingExportCommand : ICommand
    {
        public string Name => "shopping";
        public bool RequiresLogin => true;

        private readonly IEventLogger _logger;

        public ShoppingExportCommand(IEventLogger logger)
        {
            _logger = logger;
        }

        public CommandResult Execute(UserService userService, string[] args)
        {
            if (userService.CurrentUser == null)
            {
                return CommandResult.Failure("Execution context error. User is not logged in.");
            }
            if (args.Length != 2 || args[1].ToLowerInvariant() != "export")
            {
                return CommandResult.Failure("Invalid 'shopping' command format. Expected: shopping export");
            }

            var planService = userService.CurrentUser.PlanService;
            var username = userService.CurrentUser.Username;
            
            var res = planService.ExportShoppingList();

            if (res.IsSuccess)
            {
                _logger.Log("shopping_exported", new { user_name = username, items_count = 0 });
            }

            return res;
        }
    }
}