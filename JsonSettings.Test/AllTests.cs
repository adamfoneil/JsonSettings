using JsonSettings;
using JsonSettings.Library;
using JsonSettings.Test.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.IO;
using Testing.Models;

namespace Testing
{
    [TestClass]
    public class AllTests
    {
        [TestMethod]
        public void CreateAppSettings2()
        {
            var settings = SettingsBase.Load<AppSettings2>();
            Assert.IsTrue(settings != null);
        }

        [TestMethod]
        public void SetAppSettings2()
        {
            var settings = SettingsBase.Load<AppSettings2>();
            settings.Greeting = "whatever";
            settings.Save();

            settings = SettingsBase.Load<AppSettings2>();
            Assert.IsTrue(settings.Greeting.Equals("whatever"));
        }


        [TestMethod]
        public void CreateAndSaveAppScope()
        {
            var settings = JsonSettingsBase.Load<AppSettings>();
            settings.Save();
            Assert.IsTrue(settings.GetFullPath().EndsWith(@"\AppData\Local\Adam O'Neil Software\My Sample App\AppSettings.json"));
        }

        [TestMethod]
        public void CreateAndSaveUserScope()
        {
            var settings = JsonSettingsBase.Load<UserSettings>();
            settings.Save();
            Assert.IsTrue(settings.GetFullPath().EndsWith(@"\Adam O'Neil Software\My Sample App\UserSettings.json"));
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

            // when we load from disk, the sensitive value is what we started with
            settings = JsonSettingsBase.Load<AppSettings>();
            Assert.IsTrue(settings.SensitiveValue.Equals(testValue));
        }

        [TestMethod]
        public void GetValueByPath()
        {
            string json = @"{
			  ""ConnectionStrings"": {
				""DefaultConnection"": ""Data Source=(localdb)\\mssqllocaldb;Database=CloudDoc;Integrated Security=true""
			  },
			  ""Logging"": {
				""LogLevel"": {
				  ""Default"": ""Warning""
				}
			  },
			  ""AllowedHosts"": ""*""
			}";
            string connectionString = JsonConfig.GetValueFromJson<string>(json, "ConnectionStrings.DefaultConnection");
            Assert.IsTrue(connectionString.Equals("Data Source=(localdb)\\mssqllocaldb;Database=CloudDoc;Integrated Security=true"));
        }

        [TestMethod]
        public void SecureDictionaryShouldSaveAndLoad()
        {
            var sd = new SecureDictionary();
            sd.Contents["Greeting"] = "hello";
            sd.Contents["RightNow"] = DateTime.Now.ToLongDateString();

            string fileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "secureDictionary.json");
            JsonFile.Save(fileName, sd);

            sd = JsonFile.Load<SecureDictionary>(fileName);
            Assert.IsTrue(sd.Contents["Greeting"].Equals("hello"));
            Assert.IsTrue(sd.Contents["RightNow"].Equals(DateTime.Now.ToLongDateString()));
        }

        [TestMethod]
        public void MissingFileShouldCreate()
        {
            var sample = JsonFile.LoadOrCreate<SampleClass>(@"c:\non-existent-path\whatever.json");
            Assert.IsTrue(sample != null);
        }
    }
}