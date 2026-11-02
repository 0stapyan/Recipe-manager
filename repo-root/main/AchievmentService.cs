using System;
using System.Collections.Generic;
using System.Linq;
using RecipeManager.main.Achievements;

namespace RecipeManager
{
    public class AchievementService
    {
        private readonly List<IAchievement> _achievements;

        public AchievementService(IEnumerable<IAchievement> allAchievements)
        {
            _achievements = allAchievements.ToList();
        }

        public IReadOnlyList<IAchievement> GetAchievements()
        {
            return _achievements;
        }
        public void NotifyEvent(AchievementType type, object data)
        {
            foreach (var achievement in _achievements.Where(a => !a.IsUnlocked && a.TargetEvent == type))
            {
                achievement.HandleEvent(data);
            }
        }
        public IReadOnlyList<AchievementState> ExportStates()
        {
            var list = new List<AchievementState>();
            foreach (var a in _achievements)
            {
                if (a is IPersistableAchievement p)
                    list.Add(p.ExportState());
            }
            return list;
        }
        public void ImportStates(IEnumerable<AchievementState> states)
        {
            var byId = states?.ToDictionary(s => s.Id, StringComparer.Ordinal)
                       ?? new Dictionary<string, AchievementState>(StringComparer.Ordinal);

            foreach (var a in _achievements)
            {
                if (a is IPersistableAchievement p && byId.TryGetValue(p.Id, out var st))
                {
                    p.ImportState(st);
                }
            }
        }
    }
}
