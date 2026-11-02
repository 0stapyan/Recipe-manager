using System.Collections.Generic;
using System.Linq;
using System;

namespace RecipeManager
{
    public class ActionRegistry
    {
        private readonly Dictionary<string, IAction> _actions;

        public ActionRegistry(IEnumerable<IAction> actions)
        {
            _actions = actions.ToDictionary(a => a.Name, StringComparer.OrdinalIgnoreCase);
        }

        public IAction GetAction(string name)
        {
            _actions.TryGetValue(name, out var action);
            return action;
        }

        public IEnumerable<string> GetAvailableActionNames()
        {
            return _actions.Keys;
        }
    }
}