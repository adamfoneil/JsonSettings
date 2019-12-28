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

        public static T Load<T>() where T : SettingsBase, new()
        {
            T result = new T();

            if (File.Exists(result.Filename))
            {
                result = JsonFile.Load<T>(result.Filename);
            }

            return result;
        }

        public void Save(Action<JsonSerializerSettings> updateSettings = null)
        {
            JsonFile.Save(Filename, updateSettings);
        }
    }
}
