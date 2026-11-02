using System;

namespace RecipeManager.main.Achievements
{
    public sealed class CookFiveTimes : IAchievement, IPersistableAchievement
    {
        public string Name => "Master Chef Starter";
        public string Description => "Cook something 5 times.";
        public AchievementType TargetEvent => AchievementType.Cook;

        public const int TargetCount = 5;
        public int CurrentCount { get; private set; } = 0;

        public bool IsUnlocked => CurrentCount >= TargetCount;
        public string Status => IsUnlocked ? "UNLOCKED" : $"{CurrentCount}/{TargetCount}";

        public string Id => "CookFiveTimes";
        public AchievementState ExportState() => new AchievementState
        {
            Id = Id,
            IsUnlocked = IsUnlocked,
            IntProgress = CurrentCount
        };
        public void ImportState(AchievementState s)
        {
            CurrentCount = s?.IntProgress ?? 0;
        }

        public bool HandleEvent(object data)
        {
            if (IsUnlocked) return false;
            CurrentCount += 1;
            return IsUnlocked;
        }
    }
}