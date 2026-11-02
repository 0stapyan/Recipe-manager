using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using RecipeManager.main.Achievements;

namespace RecipeManager
{
    public class RecipeService
    {
        private readonly List<Recipe> _recipes = new List<Recipe>();
        private readonly PantryService _pantryService;
        
        private readonly ILimitsService _limitsService;
        private readonly IPlanTypeProvider _planTypeProvider;
        private readonly AchievementService _achievements;
        
        public RecipeService(PantryService pantryService, ILimitsService limitsService, IPlanTypeProvider planTypeProvider, AchievementService achievements) 
        {
            _pantryService = pantryService;
            _limitsService = limitsService;
            _planTypeProvider = planTypeProvider;
            _achievements = achievements;
        }

        public Recipe GetRecipe(string name)
        {
            return _recipes.FirstOrDefault(r => r.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }
        
        public List<Recipe> GetRecipes() => _recipes.OrderBy(r => r.Name).ToList();
        
        public int GetTotalRecipeCount()
        {
            return _recipes.Count; 
        }

        public CommandResult AddRecipe(string name)
        {
            name = name.Trim();
            if (string.IsNullOrWhiteSpace(name))
            {
                return CommandResult.Failure("Recipe name cannot be empty.");
            }

            if (GetRecipe(name) != null)
            {
                return CommandResult.Failure($"Recipe '{name}' already exists.");
            }
            
            PlanType currentPlan = _planTypeProvider.GetCurrentPlanType();
            PlanLimits limits = _limitsService.GetLimits(currentPlan);

            if (_recipes.Count >= limits.MaxRecipes)
            {
                return CommandResult.Failure($"Cannot add recipe '{name}'. Recipe limit of {limits.MaxRecipes} exceeded for {currentPlan} plan.");
            }

            _recipes.Add(new Recipe { Name = name });
            return CommandResult.Success($"Recipe '{name}' created.");
        }

        public CommandResult AddIngredient(string recipeName, string ingredientName, decimal quantity, string unit)
        {
            var recipe = GetRecipe(recipeName);
            if (recipe == null)
            {
                return CommandResult.Failure($"Recipe '{recipeName}' not found.");
            }
            if (quantity <= 0)
            {
                return CommandResult.Failure("Quantity must be positive.");
            }
            var existing = recipe.Ingredients.FirstOrDefault(i => 
                i.Name.Equals(ingredientName, StringComparison.OrdinalIgnoreCase) && 
                i.Unit.Equals(unit, StringComparison.OrdinalIgnoreCase));

            if (existing != null)
            {
                existing.Quantity += quantity;
                return CommandResult.Success($"Updated ingredient in '{recipeName}'. Total {existing.Name}: {existing.Quantity} {existing.Unit}.");
            }
            
            recipe.Ingredients.Add(new Ingredient
            {
                Name = ingredientName,
                Quantity = quantity,
                Unit = unit
            });
            return CommandResult.Success($"Ingredient '{ingredientName} {quantity} {unit}' added to '{recipeName}'.");
        }

        public CommandResult AddStep(string recipeName, string stepText)
        {
            var recipe = GetRecipe(recipeName);
            if (recipe == null)
            {
                return CommandResult.Failure($"Recipe '{recipeName}' not found.");
            }
            if (string.IsNullOrWhiteSpace(stepText))
            {
                return CommandResult.Failure("Step text cannot be empty.");
            }
            
            recipe.Steps.Add(stepText);
            return CommandResult.Success($"Step #{recipe.Steps.Count} added to '{recipeName}'.");
        }
        
        public CommandResult GetRecipeInfo(string recipeName)
        {
            var recipe = GetRecipe(recipeName);
            if (recipe == null)
            {
                return CommandResult.Failure($"Recipe '{recipeName}' not found.");
            }

            var info = new StringBuilder();
            info.AppendLine($"\n--- Recipe: {recipe.Name} ---");
            
            info.AppendLine("\n[ Ingredients ]");
            if (recipe.Ingredients.Any())
            {
                foreach (var i in recipe.Ingredients)
                {
                    info.AppendLine($"- {i.Name}: {i.Quantity} {i.Unit}");
                }
            }
            else
            {
                info.AppendLine("- No ingredients added yet.");
            }

            info.AppendLine("\n[ Steps ]");
            if (recipe.Steps.Any())
            {
                for (int i = 0; i < recipe.Steps.Count; i++)
                {
                    info.AppendLine($"{i + 1}. {recipe.Steps[i]}");
                }
            }
            else
            {
                info.AppendLine("- No steps added yet.");
            }

            return CommandResult.Success(info.ToString());
        }
        
        public CommandResult ScaleRecipe(string recipeName, decimal multiplier)
        {
            var recipe = GetRecipe(recipeName);
            if (recipe == null) return CommandResult.Failure($"Recipe '{recipeName}' not found.");
            if (multiplier <= 0) return CommandResult.Failure("Multiplier must be positive.");

            var info = new StringBuilder();
            info.AppendLine($"\n--- Scaled Recipe: {recipe.Name} (x{multiplier}) ---");
            
            foreach (var i in recipe.Ingredients)
            {
                var scaledQuantity = i.Quantity * multiplier;
                info.AppendLine($"- {i.Name}: {scaledQuantity} {i.Unit}");
            }
            info.AppendLine("\n[ Steps ] - Unchanged.");
            return CommandResult.Success(info.ToString());
        }
        
        public CommandResult CheckMissingIngredients(string recipeName)
        {
            var recipe = GetRecipe(recipeName);
            if (recipe == null) return CommandResult.Failure($"Recipe '{recipeName}' not found.");

            var missingIngredients = new List<string>();
            var stockList = _pantryService.GetStockInfo();
            
            foreach (var requiredIngredient in recipe.Ingredients)
            {
                var stockItem = stockList.FirstOrDefault(s => 
                    s.Name.Equals(requiredIngredient.Name, StringComparison.OrdinalIgnoreCase) &&
                    s.Unit.Equals(requiredIngredient.Unit, StringComparison.OrdinalIgnoreCase)); 

                if (stockItem == null || stockItem.Quantity < requiredIngredient.Quantity)
                {
                    missingIngredients.Add($"- {requiredIngredient.Name} (Need: {requiredIngredient.Quantity} {requiredIngredient.Unit}, Have: {(stockItem?.Quantity ?? 0)} {requiredIngredient.Unit})");
                }
            }
            
            if (missingIngredients.Any())
            {
                var message = new StringBuilder();
                message.AppendLine($"[ERROR] Cannot cook '{recipeName}'. Missing or insufficient stock:");
                message.Append(string.Join("\n", missingIngredients));
                return CommandResult.Failure(message.ToString());
            }
            else
            {
                return CommandResult.Success($"You have all ingredients required to cook '{recipeName}'.");
            }
        }
        
        public CommandResult CookRecipe(string recipeName, decimal multiplier)
        {
            var recipe = GetRecipe(recipeName);
            if (recipe == null) return CommandResult.Failure($"Recipe '{recipeName}' not found.");
            if (multiplier <= 0) return CommandResult.Failure("Multiplier must be positive.");
            
            var checkResult = CheckMissingIngredients(recipeName);
            if (!checkResult.IsSuccess) return CommandResult.Failure(checkResult.Message); 
            
            var usedIngredients = new List<string>();
            
            foreach (var requiredIngredient in recipe.Ingredients)
            {
                var quantityToUse = requiredIngredient.Quantity * multiplier;
                
                _pantryService.UseStock(
                    requiredIngredient.Name, 
                    quantityToUse, 
                    requiredIngredient.Unit, 
                    $"Used for recipe '{recipeName}' (x{multiplier})");
                    
                usedIngredients.Add($"- {requiredIngredient.Name}: {quantityToUse} {requiredIngredient.Unit}");
            }
            
            _achievements.NotifyEvent(AchievementType.Cook, new { RecipeName = recipeName, Multiplier = multiplier });
            
            var successMessage = new StringBuilder();
            successMessage.AppendLine($"Successfully cooked '{recipeName}' (x{multiplier}). Used ingredients:");
            successMessage.Append(string.Join("\n", usedIngredients));
            return CommandResult.Success(successMessage.ToString());
        }
        
        public IReadOnlyList<Recipe> ExportRecipes() => _recipes.ToList();

        public void ImportRecipes(IEnumerable<Recipe> recipes)
        {
            _recipes.Clear();
            _recipes.AddRange(recipes);
        }

    }
}