using System;
using System.Collections.Generic;
using System.Text;
using RecipeManager.Events;

namespace RecipeManager.main.Achievements
{
    public sealed class AchievementEventSubscriber : IEventSubscriber
    {
        private readonly UserService _userService;

        public AchievementEventSubscriber(UserService userService)
        {
            _userService = userService;
        }

        public void OnEvent(string eventName, object? data)
        {
            var user = _userService.CurrentUser;
            if (user == null) return;

            if (TryMapToAchievementType(eventName, out var t))
            {
                user.AchievementService.NotifyEvent(t, data ?? new { });
            }
        }
        private static bool TryMapToAchievementType(string eventName, out AchievementType result)
        {
            var candidates = new List<string> { SnakeToPascal(eventName) };
            switch (eventName)
            {
                case "recipe_ingredient":
                    candidates.Add("RecipeIngredientAdded");
                    candidates.Add("RecipeIngredient");
                    break;
                case "shopping_exported":
                    candidates.Add("ShoppingExported");
                    break;
                case "plan_changed":
                    candidates.Add("PlanChanged");
                    break;
            }

            foreach (var c in candidates)
            {
                if (Enum.TryParse<AchievementType>(c, ignoreCase: true, out result))
                    return true;
            }

            result = default;
            return false;
        }

        private static string SnakeToPascal(string s)
        {
            if (string.IsNullOrWhiteSpace(s)) return s;
            var parts = s.Split('_', StringSplitOptions.RemoveEmptyEntries);
            var sb = new StringBuilder();
            foreach (var p in parts)
            {
                if (p.Length == 0) continue;
                sb.Append(char.ToUpperInvariant(p[0]));
                if (p.Length > 1) sb.Append(p.AsSpan(1));
            }
            return sb.ToString();
        }
    }
}
