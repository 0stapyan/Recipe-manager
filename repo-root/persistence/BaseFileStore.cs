using System.IO;

namespace RecipeManager.Persistence
{
    public abstract class BaseFileStore
    {
        private readonly string _baseDir;
        private readonly string _fileName;
        
        protected BaseFileStore(string baseDir, string fileName)
        {
            _baseDir = baseDir;
            _fileName = fileName;
            Directory.CreateDirectory(_baseDir); 
        }
        
        protected string GetUserDirectoryPath(string username)
        {
            return Path.Combine(_baseDir, username);
        }
        
        protected string GetUserFilePath(string username)
        {
            return Path.Combine(GetUserDirectoryPath(username), _fileName);
        }

        public bool Exists(string username)
        {
            return File.Exists(GetUserFilePath(username));
        }

        protected void EnsureUserDirectoryExists(string username)
        {
            var dirPath = GetUserDirectoryPath(username);
            Directory.CreateDirectory(dirPath);
        }
    }
}