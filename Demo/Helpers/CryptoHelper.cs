using System;
using System.Security.Cryptography;
using System.Text;

namespace Demo.Helpers
{
    internal static class CryptoHelper
    {
        // Encrypt plaintext -> base64(nonce(12) + tag(16) + ciphertext)
        public static string EncryptStringToBase64(byte[] key, string plaintext)
        {
            if (plaintext is null) return string.Empty;
            var plainBytes = Encoding.UTF8.GetBytes(plaintext);

            var nonce = RandomNumberGenerator.GetBytes(12);
            var ciphertext = new byte[plainBytes.Length];
            var tag = new byte[16];

            using (var aes = new AesGcm(key))
            {
                aes.Encrypt(nonce, plainBytes, ciphertext, tag);
            }

            var result = new byte[nonce.Length + tag.Length + ciphertext.Length];
            Buffer.BlockCopy(nonce, 0, result, 0, nonce.Length);
            Buffer.BlockCopy(tag, 0, result, nonce.Length, tag.Length);
            Buffer.BlockCopy(ciphertext, 0, result, nonce.Length + tag.Length, ciphertext.Length);

            return Convert.ToBase64String(result);
        }

        // Decrypt from base64 format above
        public static string DecryptStringFromBase64(byte[] key, string base64)
        {
            if (string.IsNullOrEmpty(base64)) return string.Empty;
            var data = Convert.FromBase64String(base64);

            if (data.Length < 12 + 16) throw new CryptographicException("Invalid encrypted payload.");

            var nonce = new byte[12];
            var tag = new byte[16];
            var ciphertext = new byte[data.Length - nonce.Length - tag.Length];

            Buffer.BlockCopy(data, 0, nonce, 0, nonce.Length);
            Buffer.BlockCopy(data, nonce.Length, tag, 0, tag.Length);
            Buffer.BlockCopy(data, nonce.Length + tag.Length, ciphertext, 0, ciphertext.Length);

            var plainBytes = new byte[ciphertext.Length];

            using (var aes = new AesGcm(key))
            {
                aes.Decrypt(nonce, ciphertext, tag, plainBytes);
            }

            return Encoding.UTF8.GetString(plainBytes);
        }
    }
}