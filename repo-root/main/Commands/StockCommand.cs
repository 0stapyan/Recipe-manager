using System;
using System.Collections.Generic;
using System.Linq;
using RecipeManager.main.Commands.SubCommands;

namespace RecipeManager.main.Commands
{
    public class StockCommand : ICommand
    {
        public string Name => "stock";
        public bool RequiresLogin => true;
        private readonly Dictionary<string, ISubCommand> _subCommands; 

        public StockCommand(IEnumerable<ISubCommand> subCommands) 
        {
            _subCommands = subCommands.ToDictionary(c => c.Name, StringComparer.OrdinalIgnoreCase);
        }

        public CommandResult Execute(UserService userService, string[] args)
        {
            if (userService.CurrentUser == null)
            {
                return CommandResult.Failure("Execution context error. User is not logged in.");
            }
            
            var subCommandName = args.Length > 1 ? args[1].ToLowerInvariant() : string.Empty;
            
            if (_subCommands.TryGetValue(subCommandName, out var subCommand))
            {
                return subCommand.Execute(userService, args);
            }
            
            var availableSubs = string.Join(", ", _subCommands.Keys);
            return CommandResult.Failure($"Unknown 'stock' subcommand: {subCommandName}. Available: {availableSubs}");
        }
    }
}