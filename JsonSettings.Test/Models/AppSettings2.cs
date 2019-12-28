using JsonSettings.Library;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Security.Cryptography;

namespace JsonSettings.Test.Models
{
    public class AppSettings2 : SettingsBase
    {
        [JsonIgnore]
        public override string Filename 
            => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "JsonSettings.Library", "settings.json");

        public string Greeting { get; set; }

        [JsonProtect(DataProtectionScope.CurrentUser)]
        public string SensitiveValue { get; set; }

    }
}
