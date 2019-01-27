using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Cryptography;

namespace JsonSettings
{
	public class SecureDictionary
	{
		public SecureDictionary()
		{
			Contents = new Dictionary<string, string>();
		}

		private const char PairSeparator = ';';
		private const char KeyValueSeparator = ':';

		protected virtual DataProtectionScope Scope { get { return DataProtectionScope.CurrentUser; } }

		[JsonIgnore]
		public Dictionary<string, string> Contents { get; set; }

		/// <summary>
		/// Used to store the encrypted contents of your dictionary.
		/// Don't read or write to this directly
		/// </summary>
		public string SourceString { get; set; }

		[OnSerializing]
		internal void OnSerializingMethod(StreamingContext context)
		{
			string sourceString = string.Join(PairSeparator.ToString(), this.Contents.Select(kp => $"{kp.Key}{KeyValueSeparator}{kp.Value}"));
			SourceString = DataProtection.Encrypt(sourceString, Scope);
		}

		[OnDeserialized]
		internal void OnDeserializedMethod(StreamingContext context)
		{
			try
			{
				string sourceString = DataProtection.Decrypt(SourceString, Scope);

				var keyPairs = sourceString.Split(PairSeparator);
				foreach (var kp in keyPairs)
				{
					string[] parts = kp.Split(KeyValueSeparator);
					if (parts.Length == 2)
					{
						Contents.Add(parts[0].Trim(), parts[1].Trim());
					}					
				}

				SourceString = null;
			}
			catch (Exception exc)
			{
				throw new Exception($"Error deserializing SecureDictionary: {exc.Message}", exc);
			}
		}
	}
}