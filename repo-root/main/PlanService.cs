using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace RecipeManager
{
    public class PlanService
    {
        private readonly List<PlanItem> _plan = new List<PlanItem>();
        private readonly RecipeService _recipeService;
        
        private readonly ILimitsService _limitsService;
        private readonly IPlanTypeProvider _planTypeProvider;
        
        public PlanService(RecipeService recipeService, ILimitsService limitsService, IPlanTypeProvider planTypeProvider)
        {
            _recipeService = recipeService;
            _limitsService = limitsService;
            _planTypeProvider = planTypeProvider;
        }

        public int GetTotalPlanEntryCount()
        {
            return _plan.Count;
        }

        public CommandResult AddPlan(string recipeName, DateTime date, decimal multiplier = 1)
        {
            if (multiplier <= 0)
            {
                return CommandResult.Failure("Servings multiplier must be positive.");
            }
            if (_recipeService.GetRecipe(recipeName) == null)
            {
                return CommandResult.Failure($"Recipe '{recipeName}' not found. Cannot add to plan.");
            }
            
            PlanType currentPlan = _planTypeProvider.GetCurrentPlanType();
            PlanLimits limits = _limitsService.GetLimits(currentPlan);

            if (_plan.Count >= limits.MaxPlanEntries)
            {
                return CommandResult.Failure($"Cannot add plan entry. Plan entry limit of {limits.MaxPlanEntries} exceeded for {currentPlan} plan.");
            }
            
            _plan.Add(new PlanItem 
            { 
                RecipeName = recipeName, 
                Date = date.Date,
                ServingsMultiplier = multiplier 
            });
            
            return CommandResult.Success($"Recipe '{recipeName}' planned for {date.ToShortDateString()} with multiplier x{multiplier}.");
        }

        public CommandResult GetPlanList()
        {
            if (!_plan.Any())
            {
                return CommandResult.Success("No recipes are currently planned.");
            }
            
            var message = new StringBuilder("Current Cooking Plan:\n");
            var sortedPlan = _plan.OrderBy(p => p.Date);

            foreach (var item in sortedPlan)
            {
                message.AppendLine($"- {item.Date.ToShortDateString()}: {item.RecipeName} (x{item.ServingsMultiplier})");
            }
            
            return CommandResult.Success(message.ToString());
        }
        
        public CommandResult ExportShoppingList()
        {
            if (!_plan.Any())
            {
                return CommandResult.Success("Plan is empty. Nothing to shop for.");
            }
            
            var shoppingList = new Dictionary<(string Name, string Unit), decimal>();

            foreach (var planItem in _plan)
            {
                var recipe = _recipeService.GetRecipe(planItem.RecipeName);
                if (recipe == null) continue;
                
                foreach (var ingredient in recipe.Ingredients)
                {
                    var requiredAmount = ingredient.Quantity * planItem.ServingsMultiplier;
                    var key = (ingredient.Name, ingredient.Unit);

                    if (shoppingList.ContainsKey(key))
                    {
                        shoppingList[key] += requiredAmount;
                    }
                    else
                    {
                        shoppingList.Add(key, requiredAmount);
                    }
                }
            }

            if (!shoppingList.Any())
            {
                return CommandResult.Success("No ingredients found in the planned recipes.");
            }
            
            var output = new StringBuilder("--- SHOPPING LIST ---\n");
            
            foreach (var item in shoppingList.OrderBy(i => i.Key.Name))
            {
                output.AppendLine($"- {item.Key.Name}: {item.Value} {item.Key.Unit}");
            }
            
            return CommandResult.Success(output.ToString());
        }
        
        public IReadOnlyList<PlanItem> ExportPlan() => _plan.ToList();

        public void ImportPlan(IEnumerable<PlanItem> items)
        {
            _plan.Clear();
            _plan.AddRange(items);
        }

    }
}