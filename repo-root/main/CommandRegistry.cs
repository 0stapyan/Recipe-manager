using System.Collections.Generic;
using System.Linq;
using System;

namespace RecipeManager
{
    public class CommandRegistry
    {
        private readonly Dictionary<string, ICommand> _commands;

        public CommandRegistry(IEnumerable<ICommand> commands)
        {
            _commands = commands.ToDictionary(c => c.Name, StringComparer.OrdinalIgnoreCase);
        }

        public ICommand GetCommand(string name)
        {
            _commands.TryGetValue(name, out var command);
            return command;
        }
    }
}