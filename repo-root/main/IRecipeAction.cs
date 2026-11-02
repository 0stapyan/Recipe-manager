namespace RecipeManager
{
    public interface IRecipeAction
    {
        string Name { get; }
        string Description { get; }
        
        CommandResult Execute(string recipeName, string[] parameters);
    }
}