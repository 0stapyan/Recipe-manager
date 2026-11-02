using System;
using System.Globalization;
using RecipeManager.logs;
using RecipeManager.main.Commands.SubCommands;

namespace RecipeManager
{
    public class StockUseSubCommand : ISubCommand
    {
        public string Name => "use";
        private readonly IEventLogger _logger;

        public StockUseSubCommand(IEventLogger logger)
        {
            _logger = logger;
        }

        public CommandResult Execute(UserService userService, string[] args)
        {
            if (userService.CurrentUser == null)
            {
                return CommandResult.Failure("Execution context error. User is not logged in.");
            }

            if (args.Length < 5 || args.Length > 6 ||
                !decimal.TryParse(args[3], NumberStyles.Any, CultureInfo.InvariantCulture, out decimal quantity))
            {
                return CommandResult.Failure("Invalid 'stock use' format. Expected: stock use <ingredient> <quantity> <unit> [reason]");
            }

            var username = userService.CurrentUser.Username;
            var pantryService = userService.CurrentUser.PantryService;

            string name = args[2];
            string unit = args[4];
            string? reason = args.Length == 6 ? args[5] : null;

            var result = pantryService.UseStock(name, quantity, unit, reason);

            if (result.IsSuccess)
            {
                _logger.Log("stock_used", new
                {
                    user_name = username,
                    ingredient = name,
                    quantity,
                    unit,
                    reason = reason ?? ""
                });
            }
            else
            {
                if (!string.IsNullOrEmpty(result.Message) &&
                    result.Message.IndexOf("limit", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    _logger.Log("limit_reached", new { user_name = username, limit_type = "pantry_items" });
                }
            }

            return result;
        }
    }
}