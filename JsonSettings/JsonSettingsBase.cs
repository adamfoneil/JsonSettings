using Newtonsoft.Json;
using System;
using System.IO;

namespace JsonSettings
{
	public abstract class JsonSettingsBase
	{
		[JsonIgnore]
		public string Filename { get; set; }
		[JsonIgnore]
		public string CompanyName { get; set; }
		[JsonIgnore]
		public string ProductName { get; set; }

		public static T Load<T>(string fileName) where T : JsonSettingsBase, new()
		{
			T result = new T();
			string resolvedFilename = result.ResolveFilename(fileName);

			if (File.Exists(resolvedFilename))
			{
				using (StreamReader reader = File.OpenText(resolvedFilename))
				{
					JsonSerializerSettings settings = new JsonSerializerSettings()
					{						
						ContractResolver = new DataProtectionResolver()
					};

					string json = reader.ReadToEnd();
					result = JsonConvert.DeserializeObject<T>(json, settings);
				}
			}

			result.Filename = resolvedFilename;
			return result;
		}

		private string ResolveFilename(string fileName)
		{
			return (fileName.StartsWith("~")) ?
				Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), CompanyName, ProductName, fileName.Substring(1)) :
				fileName;
		}

		public void Save()
		{			
			using (StreamWriter writer = File.CreateText(Filename))
			{
				JsonSerializerSettings settings = new JsonSerializerSettings()
				{
					Formatting = Formatting.Indented,
					ContractResolver = new DataProtectionResolver()
				};
				string json = JsonConvert.SerializeObject(this, settings);
				writer.Write(json);
			}
		}
	}
}