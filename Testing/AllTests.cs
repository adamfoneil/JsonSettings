using System;
using System.IO;
using JsonSettings;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Testing.Models;

namespace Testing
{
	[TestClass]
	public class AllTests
	{
		[TestMethod]
		public void CreateAndSaveAppScope()
		{
			var settings = JsonSettingsBase.Load<AppSettings>();
			settings.Save();
			Assert.IsTrue(settings.GetFullPath().EndsWith(@"\AppData\Local\Adam O'Neil Software\My Sample App\Settings.json"));
		}

		[TestMethod]
		public void VerifyEncryptedValue()
		{
			const string testValue = "whatever";

			var settings = new AppSettings(); // will simply overwrite any existing settings
			settings.SensitiveValue = testValue;
			settings.Save();

			// inspect the json directly
			string fileName = settings.GetFullPath();
			using (StreamReader reader = File.OpenText(fileName))
			{
				string json = reader.ReadToEnd();
				AppSettings test = JsonConvert.DeserializeObject<AppSettings>(json);

				// the sensitive value on disk is not the same as in memory and not just empty
				Assert.IsTrue(!settings.SensitiveValue.Equals(test.SensitiveValue) && !string.IsNullOrEmpty(test.SensitiveValue));
			}

			settings = JsonSettingsBase.Load<AppSettings>();
			Assert.IsTrue(settings.SensitiveValue.Equals(testValue));
		}
	}
}
