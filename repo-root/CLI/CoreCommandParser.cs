using System;
using System.Linq;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Text;

namespace RecipeManager
{
    public class CoreCommandParser : ICommandParser
    {
        private readonly CommandRegistry _commandRegistry;
        private readonly ActionRegistry _actionRegistry;
        private readonly UserService _userService;

        public CoreCommandParser(CommandRegistry commandRegistry, ActionRegistry actionRegistry, UserService userService)
        {
            _commandRegistry = commandRegistry;
            _actionRegistry = actionRegistry;
            _userService = userService; 
        }
        
        private string[] SplitInput(string input)
        {
            var matches = Regex.Matches(input, @"[^\s""]+|""[^""]*""");
            return matches.Cast<Match>()
                          .Select(m => m.Value.Trim('"'))
                          .ToArray();
        }

        public CommandResult ExecuteCommand(string input)
        {
            var args = SplitInput(input);
            if (args == null || args.Length == 0) return CommandResult.Failure("Please enter a command.");
            
            var mainCommandName = args[0].ToLowerInvariant();
            
            var command = _commandRegistry.GetCommand(mainCommandName);
            
            if (command == null)
            {
                return CommandResult.Failure($"Unknown command: {mainCommandName}");
            }
            
            if (command.RequiresLogin && _userService.CurrentUser == null)
            {
                return CommandResult.Failure("Error: Please log in first using 'login <username>' command.");
            }

            return command.Execute(_userService, args);
        }
    }
}