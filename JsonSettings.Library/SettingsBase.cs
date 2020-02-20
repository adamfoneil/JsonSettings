using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace JsonSettings.Library
{
    public abstract class SettingsBase
    {
        [JsonIgnore]
        public abstract string Filename { get; }

        protected static string BuildPath(Environment.SpecialFolder folder, params string[] paths)
        {
            List<string> parts = new List<string>();
            parts.Add(Environment.GetFolderPath(folder));
            parts.AddRange(paths);
            return Path.Combine(parts.ToArray());
        }

        public static T Load<T>() where T : SettingsBase, new()
        {
            T result = new T();
            
            if (File.Exists(result.Filename))
            {
                return JsonFile.Load<T>(result.Filename) ?? result;
            }

            return result;
        }

        public void Save(Action<JsonSerializerSettings> updateSettings = null)
        {
            JsonFile.Save(Filename, this, updateSettings);
        }
    }
}
