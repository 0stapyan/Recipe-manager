using System;
using System.Linq;
using System.Collections.Generic;

using RecipeManager.main.Actions;
using RecipeManager.main.Commands;
using RecipeManager.main.Commands.SubCommands;
using RecipeManager.main.Achievements;

using RecipeManager.Persistence;
using RecipeManager.logs;
using RecipeManager.Events;   

namespace RecipeManager
{
    class Program
    {
        public static void Main(string[] args)
        {
            ILimitsService limitsService = new LimitsService();

            var allAchievements = new IAchievement[]
            {
                new CookFiveTimes(),
                new UseTenUniqueIngredients(),
                new StockOneHundredKg()
            };

            IUserStore userStore = new JsonFileUserStore("data");
            var userService = new UserService(limitsService, userStore, allAchievements);
            var publisher = new EventPublisher();
            

            var fileLogger = new FileEventLogger();
            publisher.Subscribe(new FileEventSubscriber(fileLogger));

            publisher.Subscribe(new AchievementEventSubscriber(userService));
            IEventLogger eventLogger = new PublisherEventLoggerAdapter(publisher);

            var actions = new IAction[]
            {
                new CookAction(eventLogger),
                new InfoAction(),
                new ScaleAction(),
                new MissingAction()
            };
            var actionRegistry = new ActionRegistry(actions);

            var stockSubCommands = new ISubCommand[]
            {
                new StockAddSubCommand(eventLogger),
                new StockUseSubCommand(eventLogger),
                new StockInfoSubCommand()
            };

            var recipeSubCommands = new ISubCommand[]
            {
                new RecipeAddSubCommand(eventLogger),
                new RecipeAddIngredientSubCommand(eventLogger),
                new RecipeStepsAddSubCommand(eventLogger),
                new RecipeInfoSubCommand(),
                new RecipeListSubCommand()
            };

            var planSubCommands = new ISubCommand[]
            {
                new PlanAddSubCommand(eventLogger),
                new PlanListSubCommand()
            };

            var commands = new ICommand[]
            {
                new LoginCommand(userService, eventLogger),
                new StockCommand(stockSubCommands),
                new RecipeCommand(recipeSubCommands),
                new PlanCommand(planSubCommands),
                new ShoppingExportCommand(eventLogger),
                new ChangePlanCommand(limitsService, eventLogger),
                new AchievementsCommand(),
                new ActionCommand(actionRegistry),
                new OptionsCommand(actionRegistry)
            };
            var commandRegistry = new CommandRegistry(commands);

            ICommandParser coreParser = new CoreCommandParser(commandRegistry, actionRegistry, userService);
            ICommandParser parser = coreParser;

            Console.WriteLine("Recipe Manager CLI (Enter 'exit' to quit)");

            while (true)
            {
                Console.Write("Enter command > ");
                var input = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(input)) break;
                if (input.Trim().Equals("exit", StringComparison.OrdinalIgnoreCase)) break;

                var result = parser.ExecuteCommand(input);

                if (result.IsSuccess)
                {
                    Console.WriteLine($"success {result.Message}");
                    if (userService.CurrentUser != null)
                        userService.SaveCurrent();
                }
                else
                {
                    Console.WriteLine($"erorr {result.Message}");
                }
            }
        }
    }
}
