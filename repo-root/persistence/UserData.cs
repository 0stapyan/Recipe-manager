using System.Collections.Generic;

namespace RecipeManager.Persistence
{
    public class UserData
    {
        public string Username { get; set; } = string.Empty;

        public RecipeManager.PlanType PlanType { get; set; }

        public List<RecipeManager.StockItem> Stock { get; set; } = new();
        public List<RecipeManager.Recipe> Recipes { get; set; } = new();
        public List<RecipeManager.PlanItem> Plan { get; set; } = new();
        public List<RecipeManager.main.Achievements.AchievementState> Achievements { get; set; } = new();
    }
}