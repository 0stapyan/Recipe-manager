using System;
using System.Globalization;
using RecipeManager.logs;
using RecipeManager.main.Commands.SubCommands;

namespace RecipeManager
{
    public class StockAddSubCommand : ISubCommand
    {
        public string Name => "add";
        private readonly IEventLogger _logger;

        public StockAddSubCommand(IEventLogger logger)
        {
            _logger = logger;
        }

        public CommandResult Execute(UserService userService, string[] args)
        {
            if (userService.CurrentUser == null)
            {
                return CommandResult.Failure("Execution context error. User is not logged in.");
            }

            if (args.Length != 5 ||
                !decimal.TryParse(args[3], NumberStyles.Any, CultureInfo.InvariantCulture, out decimal quantity))
            {
                return CommandResult.Failure("Invalid 'stock add' format. Expected: stock add <ingredient> <quantity> <unit>");
            }

            var username = userService.CurrentUser.Username;
            var pantryService = userService.CurrentUser.PantryService;

            string name = args[2];
            string unit = args[4];

            var result = pantryService.AddStock(name, quantity, unit);

            if (result.IsSuccess)
            {
                _logger.Log("stock_added", new { user_name = username, ingredient = name, quantity, unit });
            }

            return result;
        }
    }
}