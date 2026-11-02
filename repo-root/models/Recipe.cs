using System.Collections.Generic;

namespace RecipeManager
{
    public class Recipe
    {
        public string Name { get; set; } = string.Empty;

        public List<Ingredient> Ingredients { get; set; } = new List<Ingredient>();

        public List<string> Steps { get; set; } = new List<String>();
    }
}