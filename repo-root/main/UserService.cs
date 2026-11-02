using RecipeManager.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using RecipeManager.main.Achievements;

namespace RecipeManager
{
    public class UserService
    {
        private readonly Dictionary<string, UserProfile> _profiles = new(StringComparer.OrdinalIgnoreCase);
        private readonly ILimitsService _limitsService;
        private readonly IUserStore _userStore;
        private readonly IEnumerable<IAchievement> _allAchievements;

        public UserProfile? CurrentUser { get; private set; }

        public UserService(ILimitsService limitsService, IUserStore userStore, IEnumerable<IAchievement> allAchievements)
        {
            _limitsService = limitsService;
            _userStore = userStore;
            _allAchievements = allAchievements;
        }

        public CommandResult Login(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                return CommandResult.Failure("Username cannot be empty.");

            if (_profiles.TryGetValue(username, out var existing))
            {
                CurrentUser = existing;
                return CommandResult.Success($"Logged in as existing user '{username}'.");
            }

            if (_userStore.Exists(username))
            {
                var data = _userStore.Load(username);
                if (data == null) return CommandResult.Failure("Failed to load user data.");

                var profile = new UserProfile(username, _limitsService, _allAchievements)
                {
                    PlanType = data.PlanType
                };
                profile.PantryService.ImportStock(data.Stock);
                profile.RecipeService.ImportRecipes(data.Recipes);
                profile.PlanService.ImportPlan(data.Plan);
                profile.AchievementService.ImportStates(
                    data.Achievements ?? Enumerable.Empty<AchievementState>());

                _profiles[username] = profile;
                CurrentUser = profile;
                return CommandResult.Success($"Logged in as existing user '{username}'.");
            }
            else
            {
                var profile = new UserProfile(username, _limitsService, _allAchievements);
                _profiles[username] = profile;
                CurrentUser = profile;
                SaveCurrent();
                return CommandResult.Success($"Created and logged in as new user '{username}'.");
            }
        }

        public void SaveCurrent()
        {
            if (CurrentUser == null) return;

            var data = new Persistence.UserData
            {
                Username = CurrentUser.Username,
                PlanType = CurrentUser.PlanType,
                Stock    = CurrentUser.PantryService.ExportStock().ToList(),
                Recipes  = CurrentUser.RecipeService.ExportRecipes().ToList(),
                Plan     = CurrentUser.PlanService.ExportPlan().ToList(),

                Achievements = CurrentUser.AchievementService.ExportStates().ToList()
            };

            _userStore.Save(data);
        }

        public CommandResult Logout()
        {
            if (CurrentUser == null)
                return CommandResult.Failure("No user is currently logged in.");

            var username = CurrentUser.Username;
            SaveCurrent();
            CurrentUser = null;
            return CommandResult.Success($"User '{username}' logged out.");
        }
    }
}
