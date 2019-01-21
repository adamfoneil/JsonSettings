using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;

namespace JsonSettings
{
	public static class JsonConfig
	{
		public static T GetValue<T>(string fileName, string path)
		{
			string content = File.ReadAllText(fileName);
			return GetValueFromJson<T>(content, path);
		}

		public static T GetValueFromJson<T>(string json, string path)
		{
			var token = JsonConvert.DeserializeObject<JToken>(json);
			var value = token.SelectToken(path);
			return value.Value<T>();
		}
	}
}