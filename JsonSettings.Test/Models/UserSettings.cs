using JsonSettings;
using System;

namespace Testing.Models
{
    public class UserSettings : JsonSettingsBase
    {
        public override string Filename => "UserSettings.json";
        public override Scope Scope => Scope.User;
        public override string CompanyName => "Adam O'Neil Software";
        public override string ProductName => "My Sample App";

        public string Color1 { get; set; }
        public string Color2 { get; set; }
        public DateTime SomeDate { get; set; }
    }
}