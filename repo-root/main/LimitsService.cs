using System.Collections.Generic;

namespace RecipeManager
{
    public class LimitsService : ILimitsService
    {
        private readonly Dictionary<PlanType, PlanLimits> _limits = new Dictionary<PlanType, PlanLimits>();

        public LimitsService()
        {
            _limits[PlanType.Basic] = new PlanLimits
            {
                MaxPantryItems = 15,
                MaxRecipes = 10,
                MaxPlanEntries = 30
            };
            
            _limits[PlanType.Chef] = new PlanLimits
            {
                MaxPantryItems = 200,
                MaxRecipes = 150,
                MaxPlanEntries = 500
            };
        }

        public PlanLimits GetLimits(PlanType planType)
        {
            return _limits.GetValueOrDefault(planType) ?? _limits[PlanType.Basic];
        }
    }
}