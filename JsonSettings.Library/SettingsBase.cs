using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace JsonSettings.Library
{
    public abstract class SettingsBase
    {
        [JsonIgnore]
        public abstract string Filename { get; }

        protected static string BuildPath(Environment.SpecialFolder folder, params string[] paths)
        {
            List<string> parts = new List<string>
            {
                Environment.GetFolderPath(folder)
            };
            parts.AddRange(paths);
            return Path.Combine(parts.ToArray());
        }

        protected virtual void Initialize() { }

        public static T Load<T>(EventHandler<Newtonsoft.Json.Serialization.ErrorEventArgs> error = null) where T : SettingsBase, new()
        {
            T result = new T();

            if (File.Exists(result.Filename))
            {
                result = JsonFile.Load<T>(result.Filename, error: error) ?? result;
            }

            result.Initialize();

            return result;
        }

        public void Save(Action<JsonSerializerSettings> updateSettings = null, EventHandler<Newtonsoft.Json.Serialization.ErrorEventArgs> error = null)
        {
            JsonFile.Save(Filename, this, updateSettings, error);
        }
    }
}
