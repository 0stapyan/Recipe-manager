using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RecipeManager.main.Achievements;

namespace RecipeManager
{
    public class AchievementParserDecorator : ICommandParser
    {
        private readonly ICommandParser _decoratedParser;
        private readonly UserService _userService;

        public AchievementParserDecorator(ICommandParser decoratedParser, UserService userService)
        {
            _decoratedParser = decoratedParser;
            _userService = userService;
        }

        public CommandResult ExecuteCommand(string input)
        {
            var userProfile = _userService.CurrentUser;
            List<string> initiallyUnlockedNames = null;
            
            if (userProfile != null)
            {
                initiallyUnlockedNames = userProfile.AchievementService.GetAchievements()
                    .Where(a => a.IsUnlocked)
                    .Select(a => a.Name)
                    .ToList();
            }
            
            CommandResult result = _decoratedParser.ExecuteCommand(input);
            
            if (!result.IsSuccess || userProfile == null) 
                return result;

            var currentlyUnlocked = userProfile.AchievementService.GetAchievements().Where(a => a.IsUnlocked);
                
            var newlyUnlocked = currentlyUnlocked
                .Where(a => initiallyUnlockedNames != null && !initiallyUnlockedNames.Contains(a.Name))
                .ToList();

            if (newlyUnlocked.Any())
            {
                var achievementMessage = new StringBuilder();
                achievementMessage.AppendLine("\n*** NEW ACHIEVEMENT UNLOCKED! ***");
                foreach (var ach in newlyUnlocked)
                {
                    achievementMessage.AppendLine($"- {ach.Name}: {ach.Description}");
                }
                achievementMessage.AppendLine("*********************************");
                    
                return CommandResult.Success(result.Message + achievementMessage.ToString());
            }
            
            return result;
        }
    }
}