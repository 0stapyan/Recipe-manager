using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace RecipeManager
{
    public class PersistenceService
    {
        private const string DataFileName = "user_data.json";
        private readonly JsonSerializerOptions _serializerOptions = new JsonSerializerOptions 
        { 
            WriteIndented = true,
            IncludeFields = true 
        };
        
        public async Task SaveProfilesAsync(Dictionary<string, UserProfile> profiles)
        {
            try
            {
                string jsonString = JsonSerializer.Serialize(profiles, _serializerOptions);
                await File.WriteAllTextAsync(DataFileName, jsonString);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Failed to save data: {ex.Message}");
            }
        }
        
        public async Task<Dictionary<string, UserProfile>> LoadProfilesAsync()
        {
            if (!File.Exists(DataFileName))
            {
                return new Dictionary<string, UserProfile>(StringComparer.OrdinalIgnoreCase);
            }

            try
            {
                string jsonString = await File.ReadAllTextAsync(DataFileName);
                
                var profiles = JsonSerializer.Deserialize<Dictionary<string, UserProfile>>(jsonString, _serializerOptions);

                return profiles ?? new Dictionary<string, UserProfile>(StringComparer.OrdinalIgnoreCase);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Failed to load data. Starting with empty profiles. Error: {ex.Message}");
                return new Dictionary<string, UserProfile>(StringComparer.OrdinalIgnoreCase);
            }
        }
    }
}