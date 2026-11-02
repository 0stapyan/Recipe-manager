using System;
using System.Linq;
using System.Text;

namespace RecipeManager
{
    public class AchievementsCommand : ICommand
    {
        public string Name => "achievements";
        public bool RequiresLogin => true;

        public CommandResult Execute(UserService userService, string[] args)
        {
            if (userService.CurrentUser == null)
            {
                return CommandResult.Failure("Execution context error. User is not logged in.");
            }

            if (args.Length != 2 || args[1].ToLowerInvariant() != "list")
            {
                return CommandResult.Failure("Invalid 'achievements' format. Expected: achievements list");
            }

            var achievements = userService.CurrentUser.AchievementService.GetAchievements();

            if (!achievements.Any())
            {
                return CommandResult.Success("No achievements defined.");
            }

            var sb = new StringBuilder("--- Your Achievements ---\n");

            foreach (var a in achievements)
            {
                sb.AppendLine($"- [{a.Status.PadRight(10)}] {a.Name}: {a.Description}");
            }

            return CommandResult.Success(sb.ToString());
        }
    }
}