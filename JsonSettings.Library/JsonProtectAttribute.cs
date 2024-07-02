using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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

            foreach (JsonProperty prop in props.Where(p => p.PropertyType.Equals(typeof(string))))
            {
                PropertyInfo pi = type.GetProperty(prop.UnderlyingName);
                var attr = pi.GetCustomAttribute<JsonProtectAttribute>();
                if (attr != null) prop.ValueProvider = new ProtectedDataValueProvider(pi, attr.Scope);
            }

            return props;
        }
    }

    public class ProtectedDataValueProvider : IValueProvider
    {
        public ProtectedDataValueProvider(PropertyInfo propertyInfo, DataProtectionScope scope)
        {
            PropertyInfo = propertyInfo;
            Scope = scope;
        }

        public PropertyInfo PropertyInfo { get; private set; }
        public DataProtectionScope Scope { get; private set; }

        /// <summary>
        /// Called during deserialization to decrypt a property value
        /// </summary>		
        public object GetValue(object target)
        {
            string clearText = PropertyInfo.GetValue(target) as string;
            if (!string.IsNullOrEmpty(clearText)) return DataProtection.Encrypt(clearText, Scope);
            return null;
        }

        /// <summary>
        /// Called during serialization to encrypt a property value
        /// </summary>
        /// <param name="target">The target to set the value on.</param>
        /// <param name="value">The value to set on the target.</param>
        /// <exception cref="FormatException">Unable to decrypt {PropertyInfo.Name}, from {PropertyInfo.DeclaringType.Name}. {ex.Message}</exception>
        public void SetValue(object target, object value)
        {
            string encryptedText = value as string;
            if (!string.IsNullOrEmpty(encryptedText))
            {
                string decryptedText = string.Empty;

                try {
                    decryptedText = DataProtection.Decrypt(encryptedText, Scope);
                } catch (FormatException ex) {
                    throw new FormatException($"Unable to decrypt {PropertyInfo.Name}, from {PropertyInfo.DeclaringType.Name}. {ex.Message}", ex);
                }

                PropertyInfo.SetValue(target, decryptedText);
            }
        }
    }
}