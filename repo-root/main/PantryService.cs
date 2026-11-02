using System;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;
using System.Text;
using RecipeManager.main.Achievements;

namespace RecipeManager
{
    public class PantryService
    {
        private readonly List<StockItem> _stock = new List<StockItem>();
        
        private readonly ILimitsService _limitsService;
        private readonly IPlanTypeProvider _planTypeProvider;
        private readonly AchievementService _achievements;
        
        public PantryService(ILimitsService limitsService, IPlanTypeProvider planTypeProvider, AchievementService achievements)
        {
            _limitsService = limitsService;
            _planTypeProvider = planTypeProvider;
            _achievements = achievements;
        }

        public int GetTotalItemCount()
        {
            return _stock.Count; 
        }

        public CommandResult AddStock(string name, decimal quantity, string unit)
        {
            if (quantity <= 0)
            {
                return CommandResult.Failure("Quantity must be a positive number.");
            }
            
            name = name.Trim();
            unit = unit.Trim().ToLowerInvariant();
            
            var existingItemInUnit = _stock.FirstOrDefault(i => 
                i.Name.Equals(name, StringComparison.OrdinalIgnoreCase) && 
                i.Unit.Equals(unit, StringComparison.OrdinalIgnoreCase));
            
            var existingItemAnyUnit = _stock.FirstOrDefault(i => 
                i.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
            
            bool isNewUniqueIngredient = existingItemAnyUnit == null;
            
            if (existingItemInUnit == null)
            {
                PlanType currentPlan = _planTypeProvider.GetCurrentPlanType();
                PlanLimits limits = _limitsService.GetLimits(currentPlan);
                
                if (_stock.Count >= limits.MaxPantryItems)
                {
                    return CommandResult.Failure($"Cannot add '{name}'. Pantry item limit of {limits.MaxPantryItems} exceeded for {currentPlan} plan.");
                }
            }
            
            decimal totalQuantity;

            if (existingItemInUnit != null)
            {
                existingItemInUnit.Quantity += quantity;
                existingItemInUnit.LastUpdated = DateTime.Now;
                totalQuantity = existingItemInUnit.Quantity; 
            }
            else
            {
                var newItem = new StockItem
                {
                    Name = name,
                    Quantity = quantity,
                    Unit = unit,
                    LastUpdated = DateTime.Now
                };
                _stock.Add(newItem);
                totalQuantity = newItem.Quantity;
            }
            
            if (isNewUniqueIngredient)
            {
                _achievements.NotifyEvent(AchievementType.UseUniqueIngredient, name);
            }
            
            _achievements.NotifyEvent(AchievementType.Stock, new { Quantity = quantity, Unit = unit });
            

            return CommandResult.Success($"Stock updated. Total: {totalQuantity} {unit}."); 
        }
        
        public CommandResult UseStock(string name, decimal quantity, string unit, string reason = null)
        {
            if (quantity <= 0)
            {
                return CommandResult.Failure("Quantity must be a positive number for 'use'.");
            }

            name = name.Trim();
            unit = unit.Trim().ToLowerInvariant();

            var existingItem = _stock.FirstOrDefault(i => 
                i.Name.Equals(name, StringComparison.OrdinalIgnoreCase) && 
                i.Unit.Equals(unit, StringComparison.OrdinalIgnoreCase));

            if (existingItem == null)
            {
                return CommandResult.Failure($"Ingredient '{name}' with unit '{unit}' not found in stock.");
            }

            if (existingItem.Quantity < quantity)
            {
                return CommandResult.Failure($"Insufficient stock. Available: {existingItem.Quantity} {unit}. Cannot use {quantity} {unit}.");
            }
            
            existingItem.Quantity -= quantity;
            existingItem.LastUpdated = DateTime.Now;
            
            if (existingItem.Quantity == 0)
            {
                _stock.Remove(existingItem);
                return CommandResult.Success($"Used all {quantity} {unit} of '{name}' ({reason ?? "no reason specified"}). Item removed from stock.");
            }
            
            string successMessage = $"Used {quantity} {unit} of '{name}' ({reason ?? "no reason specified"}). Remaining: {existingItem.Quantity} {unit}.";
            return CommandResult.Success(successMessage);
        }

        public List<StockItem> GetStockInfo()
        {
            return _stock.OrderBy(i => i.Name).ToList();
        }
        
        public IReadOnlyList<StockItem> ExportStock() => _stock.ToList();

        public void ImportStock(IEnumerable<StockItem> items)
        {
            _stock.Clear();
            _stock.AddRange(items);
        }

    }
    
}