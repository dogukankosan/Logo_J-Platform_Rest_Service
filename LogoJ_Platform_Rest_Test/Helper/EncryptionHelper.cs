using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
namespace LogoJ_Platform_Rest_Test.Helper
{
    internal static class EncryptionHelper
    {
        private static readonly byte[] keyBytes = Encoding.UTF8.GetBytes("12345678901234567890123456789012");
        private static readonly byte[] ivBytes = Encoding.UTF8.GetBytes("1234567890123456");
        internal static string Encrypt(string plainText)
        {
            try
            {
                using (Aes aesAlg = Aes.Create())
                {
                    aesAlg.Key = keyBytes;
                    aesAlg.IV = ivBytes;
                    using (MemoryStream msEncrypt = new MemoryStream())
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, aesAlg.CreateEncryptor(), CryptoStreamMode.Write))
                    using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                    {
                        swEncrypt.Write(plainText);
                        swEncrypt.Flush();
                        csEncrypt.FlushFinalBlock();
                        return Convert.ToBase64String(msEncrypt.ToArray());
                    }
                }
            }
            catch (Exception ex)
            {
                _ = TextLog.TextLoggingAsync($"Şifreleme hatası: {ex.Message}");
                return null;
            }
        }
        internal static string Decrypt(string cipherText)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(cipherText))
                    return null;
                byte[] cipherBytes = Convert.FromBase64String(cipherText);
                using (Aes aesAlg = Aes.Create())
                {
                    aesAlg.Key = keyBytes;
                    aesAlg.IV = ivBytes;
                    using (MemoryStream msDecrypt = new MemoryStream(cipherBytes))
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, aesAlg.CreateDecryptor(), CryptoStreamMode.Read))
                    using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        return srDecrypt.ReadToEnd();
                }
            }
            catch (Exception ex)
            {
                _ = TextLog.TextLoggingAsync($"Şifre çözme hatası: {ex.Message}");
                return null;
            }
        }
    }
}