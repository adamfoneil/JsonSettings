using JsonSettings;
using System.Security.Cryptography;

namespace Testing.Models
{
	public class Settings : JsonSettingsBase
	{
		public override string CompanyName => "Adam O'Neil Software";
		public override string ProductName => "My Sample App";

		public string Greeting { get; set; } = "hello";

		[JsonProtect(DataProtectionScope.CurrentUser)]
		public string SensitiveValue { get; set; }
	}
}