using System;
using System.IO;
using System.Text.Json;

namespace RecipeManager.Persistence
{
    public class JsonFileUserStore : BaseFileStore, IUserStore
    {
        private readonly JsonSerializerOptions _jsonOptions = new()
        {
            WriteIndented = true
        };

        public JsonFileUserStore(string baseDir = "data") : base(baseDir, "profile.json")
        {
        }

        public UserData? Load(string username)
        {
            var path = GetUserFilePath(username); 
            
            if (!File.Exists(path)) return null; 

            var json = File.ReadAllText(path);
            return JsonSerializer.Deserialize<UserData>(json, _jsonOptions);
        }

        public void Save(UserData data)
        {
            EnsureUserDirectoryExists(data.Username); 
            var path = GetUserFilePath(data.Username); 
            
            var json = JsonSerializer.Serialize(data, _jsonOptions);
            File.WriteAllText(path, json);
        }
    }
}