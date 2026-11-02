using System.Collections.Generic;

namespace RecipeManager
{
    public enum AchievementType { Cook, UseUniqueIngredient, Stock }

    public interface IAchievement
    {
        string Name { get; }
        string Description { get; }
        bool IsUnlocked { get; }
        string Status { get; }
        AchievementType TargetEvent { get; }
        
        bool HandleEvent(object data);
    }
}