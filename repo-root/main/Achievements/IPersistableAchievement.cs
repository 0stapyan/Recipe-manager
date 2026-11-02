namespace RecipeManager.main.Achievements
{
    public interface IPersistableAchievement : IAchievement
    {
        string Id { get; }
        AchievementState ExportState();
        void ImportState(AchievementState state);
    }
}