using System;
using System.Security.Cryptography;
using System.Text;

namespace JsonSettings
{
	public static class Encryption
	{
		public static string Encrypt(string clearText, DataProtectionScope scope = DataProtectionScope.CurrentUser)
		{
			byte[] clearBytes = Encoding.UTF8.GetBytes(clearText);
			byte[] encryptedBytes = ProtectedData.Protect(clearBytes, null, scope);
			return Convert.ToBase64String(encryptedBytes);
		}

		public static string Decrypt(string encryptedText, DataProtectionScope scope = DataProtectionScope.CurrentUser)
		{
			byte[] encryptedBytes = Convert.FromBase64String(encryptedText);
			byte[] clearBytes = ProtectedData.Unprotect(encryptedBytes, null, scope);
			return Encoding.UTF8.GetString(clearBytes);
		}
	}
}