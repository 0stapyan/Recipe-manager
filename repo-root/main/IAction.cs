using System;

namespace RecipeManager
{
    public interface IAction
    {
        string Name { get; } 
        
        CommandResult Execute(string username, RecipeService recipeService, string recipeName, string[] parameters);
    }
}