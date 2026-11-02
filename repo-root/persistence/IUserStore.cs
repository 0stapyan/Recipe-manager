namespace RecipeManager.Persistence
{
    public interface IUserStore
    {
        bool Exists(string username);
        UserData? Load(string username);
        void Save(UserData data);
    }
}