using System;
using System.Collections.Generic;
using RecipeManager.main.Achievements;

namespace RecipeManager
{
    public class UserProfile : IPlanTypeProvider
    {
        public string Username { get; }
        public PlanType PlanType { get; set; } = PlanType.Basic;
        
        public AchievementService AchievementService { get; }
        public PantryService PantryService { get; }
        public RecipeService RecipeService { get; }
        public PlanService PlanService { get; }
        
        public UserProfile(string username, ILimitsService limitsService, IEnumerable<IAchievement> initialAchievements) 
        {
            Username = username;
            
            AchievementService = new AchievementService(initialAchievements);
            
            PantryService = new PantryService(limitsService, this, AchievementService); 
            RecipeService = new RecipeService(PantryService, limitsService, this, AchievementService); 
            PlanService = new PlanService(RecipeService, limitsService, this);
        }
        
        public PlanType GetCurrentPlanType()
        {
            return this.PlanType;
        }
    }
}