using System;

namespace RecipeManager
{
    public class PlanItem
    {
        public string RecipeName { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public decimal ServingsMultiplier { get; set; } = 1;
    }
}