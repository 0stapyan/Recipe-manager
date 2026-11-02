using System;
using System.Linq;
using RecipeManager.main.Commands.SubCommands;

namespace RecipeManager
{
    public class StockInfoSubCommand : ISubCommand
    {
        public string Name => "info";

        public StockInfoSubCommand() { }

        public CommandResult Execute(UserService userService, string[] args)
        {
            if (userService.CurrentUser == null)
            {
                return CommandResult.Failure("Execution context error. User is not logged in.");
            }

            if (args.Length != 2)
                return CommandResult.Failure("Invalid 'stock info' format. Expected: stock info");

            var pantryService = userService.CurrentUser.PantryService;

            var stockList = pantryService.GetStockInfo();
            if (!stockList.Any()) return CommandResult.Success("Pantry is empty.");

            var message = "Current Stock:\n" +
                          string.Join("\n", stockList.Select(s =>
                              $"- {s.Name}: {s.Quantity} {s.Unit} (Updated: {s.LastUpdated:dd.MM.yy})"));
            return CommandResult.Success(message);
        }
    }
}