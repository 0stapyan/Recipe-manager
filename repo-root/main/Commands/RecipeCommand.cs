using System;
using System.Collections.Generic;
using System.Linq;
using RecipeManager.main.Commands.SubCommands;

namespace RecipeManager.main.Commands
{
    public class RecipeCommand : ICommand
    {
        public string Name => "recipe";
        public bool RequiresLogin => true;
        private readonly Dictionary<string, ISubCommand> _subCommands;
        
        public RecipeCommand(IEnumerable<ISubCommand> subCommands)
        {
            _subCommands = subCommands
                .Where(c => c.Name != "steps")
                .ToDictionary(c => c.Name, StringComparer.OrdinalIgnoreCase);
            
            var stepsCommand = subCommands.FirstOrDefault(c => c.Name == "steps");
            if (stepsCommand != null)
            {
                _subCommands["steps"] = stepsCommand;
            }
        }

        public CommandResult Execute(UserService userService, string[] args)
        {
            if (userService.CurrentUser == null)
            {
                return CommandResult.Failure("Execution context error. User is not logged in.");
            }
            
            var subCommandName = args.Length > 1 ? args[1].ToLowerInvariant() : string.Empty;
            
            if (subCommandName == "options" || subCommandName == "action")
            {
                return CommandResult.Failure($"Command '{subCommandName}' must be executed without the 'recipe' prefix, or is not recognized here.");
            }

            if (_subCommands.TryGetValue(subCommandName, out var subCommand))
            {
                return subCommand.Execute(userService, args);
            }

            var availableSubs = string.Join(", ", _subCommands.Keys);
            return CommandResult.Failure($"Unknown 'recipe' subcommand: {subCommandName}. Available: {availableSubs}");
        }
    }
}