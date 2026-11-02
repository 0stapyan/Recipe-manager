using System.Collections.Generic;

namespace RecipeManager.main.Achievements
{
    public sealed class AchievementState
    {
        public string Id { get; set; } = "";
        public bool IsUnlocked { get; set; }
        public int IntProgress { get; set; }
        public decimal DecimalProgress { get; set; }
        public List<string> Items { get; set; } = new();
    }
}