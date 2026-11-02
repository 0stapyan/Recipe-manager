using System;
using System.Collections.Generic;
using System.Linq;

namespace RecipeManager.main.Achievements
{
    public sealed class UseTenUniqueIngredients : IAchievement, IPersistableAchievement
    {
        public string Name => "The Pantry Collector";
        public string Description => "Use 10 unique ingredients.";
        public AchievementType TargetEvent => AchievementType.UseUniqueIngredient;

        public const int TargetCount = 10;

        private readonly HashSet<string> _unique = new(StringComparer.OrdinalIgnoreCase);

        public bool IsUnlocked => _unique.Count >= TargetCount;
        public string Status => IsUnlocked ? "UNLOCKED" : $"{_unique.Count}/{TargetCount}";

        public string Id => "UseTenUniqueIngredients";
        public AchievementState ExportState() => new AchievementState
        {
            Id = Id,
            IsUnlocked = IsUnlocked,
            Items = _unique.ToList(),
            IntProgress = _unique.Count
        };
        public void ImportState(AchievementState s)
        {
            _unique.Clear();
            if (s?.Items != null)
                foreach (var it in s.Items)
                    if (!string.IsNullOrWhiteSpace(it)) _unique.Add(it);
        }

        public bool HandleEvent(object data)
        {
            if (IsUnlocked) return false;

            if (data is string ingredient && !string.IsNullOrWhiteSpace(ingredient))
            {
                _unique.Add(ingredient.Trim());
            }
            return IsUnlocked;
        }
    }
}