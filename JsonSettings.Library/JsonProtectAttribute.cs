using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;

namespace JsonSettings
{
    // help from https://stackoverflow.com/a/29240043/2023653

    [AttributeUsage(AttributeTargets.Property)]
    public class JsonProtectAttribute : Attribute
    {
        public JsonProtectAttribute(DataProtectionScope scope)
        {
            Scope = scope;
        }

        public DataProtectionScope Scope { get; private set; }
    }

    public class DataProtectionResolver : DefaultContractResolver
    {
        protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
        {
            var props = base.CreateProperties(type, memberSerialization);

            foreach (JsonProperty prop in props.Where(p => !p.Ignored && p.PropertyType.Equals(typeof(string))))
            {
                if (prop.AttributeProvider
                    .GetAttributes(typeof(JsonProtectAttribute), false)
                    .FirstOrDefault() is JsonProtectAttribute attr)
                {
                    prop.ValueProvider = new ProtectedDataValueProvider(prop.ValueProvider, attr.Scope, prop.UnderlyingName, prop.DeclaringType);
                }
            }

            return props;
        }
    }

    public class ProtectedDataValueProvider : IValueProvider
    {
        public ProtectedDataValueProvider(IValueProvider valueProvider, DataProtectionScope scope, string underlyingName, Type declaringType)
        {
            UnderlyingValueProvider = valueProvider;
            Scope = scope;
            UnderlyingName = underlyingName;
            DeclaringType = declaringType;
        }

        public IValueProvider UnderlyingValueProvider { get; }
        public DataProtectionScope Scope { get; }
        public string UnderlyingName { get; }
        public Type DeclaringType { get; }

        /// <summary>
        /// Called during deserialization to decrypt a property value
        /// </summary>		
        public object GetValue(object target)
        {
            string clearText = UnderlyingValueProvider.GetValue(target) as string;
            if (!string.IsNullOrEmpty(clearText)) return DataProtection.Encrypt(clearText, Scope);
            return null;
        }

        /// <summary>
        /// Called during serialization to encrypt a property value
        /// </summary>
        /// <param name="target">The target to set the value on.</param>
        /// <param name="value">The value to set on the target.</param>
        /// <exception cref="FormatException">Unable to decrypt {underlyingName}, from {declaringType.Name}. {ex.Message}</exception>
        public void SetValue(object target, object value)
        {
            string encryptedText = value as string;
            if (!string.IsNullOrEmpty(encryptedText))
            {
                string decryptedText = string.Empty;

                try
                {
                    decryptedText = DataProtection.Decrypt(encryptedText, Scope);
                }
                catch (FormatException ex)
                {
                    throw new FormatException($"Unable to decrypt {UnderlyingName}, from {DeclaringType.Name}. {ex.Message}", ex);
                }

                UnderlyingValueProvider.SetValue(target, decryptedText);
            }
        }
    }
}