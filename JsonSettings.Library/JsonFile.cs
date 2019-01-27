using Newtonsoft.Json;
using System.IO;
using System.Threading.Tasks;

namespace JsonSettings
{
	public static class JsonFile
	{
		public static void Save<T>(string fileName, T @object)
		{
			string folder = Path.GetDirectoryName(fileName);
			if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);

			using (StreamWriter writer = File.CreateText(fileName))
			{
				JsonSerializerSettings settings = new JsonSerializerSettings()
				{
					Formatting = Formatting.Indented,
					ContractResolver = new DataProtectionResolver()
				};
				string json = JsonConvert.SerializeObject(@object, settings);
				writer.Write(json);
			}
		}

		public async static Task SaveAsync<T>(string fileName, T @object)
		{
			string folder = Path.GetDirectoryName(fileName);
			if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);

			using (StreamWriter writer = File.CreateText(fileName))
			{
				JsonSerializerSettings settings = new JsonSerializerSettings()
				{
					Formatting = Formatting.Indented,
					ContractResolver = new DataProtectionResolver()
				};
				string json = JsonConvert.SerializeObject(@object, settings);
				await writer.WriteAsync(json);
			}
		}

		public static T LoadOrCreate<T>(string fileName) where T : new()
		{
			if (!File.Exists(fileName)) return new T();

			return Load<T>(fileName);
		}

		public static T Load<T>(string fileName)
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
					return JsonConvert.DeserializeObject<T>(json, settings);
				}
			}
			return default(T);
		}

		public async static Task<T> LoadOrCreateAsync<T>(string fileName) where T : new()
		{
			if (!File.Exists(fileName)) return new T();

			return await LoadAsync<T>(fileName);
		}

		public async static Task<T> LoadAsync<T>(string fileName)
		{			
			using (StreamReader reader = File.OpenText(fileName))
			{
				JsonSerializerSettings settings = new JsonSerializerSettings()
				{
					ContractResolver = new DataProtectionResolver()
				};

				string json = await reader.ReadToEndAsync();
				if (!json.Equals(string.Empty))
				{
					return JsonConvert.DeserializeObject<T>(json, settings);
				}
			}
			return default(T);
		}
	}
}