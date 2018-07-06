using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace JsonSettings
{
	public enum Scope
	{
		Application,
		User
	}

	public abstract class JsonSettingsBase
	{
		[JsonIgnore]
		public abstract Scope Scope { get; }
		[JsonIgnore]
		public abstract string CompanyName { get; }
		[JsonIgnore]
		public abstract string ProductName { get; }
		[JsonIgnore]
		public abstract string Filename { get; }

		public static T Load<T>() where T : JsonSettingsBase, new()
		{
			T result = new T();

			string fileName = result.GetFullPath();
			if (File.Exists(fileName))
			{
				using (StreamReader reader = File.OpenText(fileName))
				{
					JsonSerializerSettings settings = new JsonSerializerSettings()
					{						
						ContractResolver = new DataProtectionResolver()
					};

					string json = reader.ReadToEnd();
					if (!json.Equals(string.Empty))
					{
						result = JsonConvert.DeserializeObject<T>(json, settings);
					}
				}
			}

			return result;
		}

		public string GetFullPath()
		{
			Dictionary<Scope, Environment.SpecialFolder> paths = new Dictionary<Scope, Environment.SpecialFolder>()
			{
				{ Scope.Application, Environment.SpecialFolder.LocalApplicationData },
				{ Scope.User, Environment.SpecialFolder.Personal }
			};

			string result = Path.Combine(Environment.GetFolderPath(paths[Scope]), CompanyName, ProductName, Filename);

			string folder = Path.GetDirectoryName(result);
			if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);

			return result;
		}

		public void Save()
		{			
			using (StreamWriter writer = File.CreateText(GetFullPath()))
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