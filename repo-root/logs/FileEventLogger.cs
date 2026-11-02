using System;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Collections.Generic;
using System.Reflection;

namespace RecipeManager.logs
{
    public class FileEventLogger : IEventLogger
    {
        private readonly string LogDir;
        private readonly string LogPath;
        private readonly object Sync = new();

        public FileEventLogger()
        {
            var root = FindProjectRoot(Directory.GetCurrentDirectory(), "RecipeManager") ?? Directory.GetCurrentDirectory();

            LogDir = Path.Combine(root, "logs");
            LogPath = Path.Combine(LogDir, "events.log");

            Directory.CreateDirectory(LogDir);
        }

        public void Log(string eventName, object? @params = null)
        {
            var dict = ToDict(@params);

            var json = JsonSerializer.Serialize(new
            {
                @event = eventName,
                timestamp = DateTime.UtcNow.ToString("o"),
                @params = dict
            });

            lock (Sync)
            {
                File.AppendAllText(LogPath, json + Environment.NewLine, Encoding.UTF8);
            }
        }

        private static Dictionary<string, object?> ToDict(object? src)
        {
            var d = new Dictionary<string, object?>(StringComparer.Ordinal);
            if (src == null) return d;
            foreach (var p in src.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
                d[p.Name] = p.GetValue(src);
            return d;
        }

        private static string? FindProjectRoot(string startDir, string projectFolderName)
        {
            var dir = new DirectoryInfo(startDir);
            while (dir != null)
            {
                if (dir.Name.Equals(projectFolderName, StringComparison.Ordinal))
                    return dir.FullName;
                dir = dir.Parent;
            }
            return null;
        }
    }
}
