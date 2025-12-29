using System;
using System.Security.Cryptography;

namespace Demo.Helpers
{
    internal static class PasswordHelper
    {
        private const int SaltSize = 16;
        private const int HashSize = 32;
        private const int Iterations = 100_000;

        // returns salt||hash
        public static byte[] HashPassword(string password)
        {
            if (password is null) throw new ArgumentNullException(nameof(password));

            var salt = RandomNumberGenerator.GetBytes(SaltSize);
            using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, Iterations, HashAlgorithmName.SHA256);
            var hash = pbkdf2.GetBytes(HashSize);

            var result = new byte[SaltSize + HashSize];
            Buffer.BlockCopy(salt, 0, result, 0, SaltSize);
            Buffer.BlockCopy(hash, 0, result, SaltSize, HashSize);
            return result;
        }

        public static bool VerifyPassword(byte[] storedSaltHash, string password)
        {
            if (storedSaltHash is null || storedSaltHash.Length != SaltSize + HashSize) return false;
            var salt = new byte[SaltSize];
            var hash = new byte[HashSize];
            Buffer.BlockCopy(storedSaltHash, 0, salt, 0, SaltSize);
            Buffer.BlockCopy(storedSaltHash, SaltSize, hash, 0, HashSize);

            using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, Iterations, HashAlgorithmName.SHA256);
            var computed = pbkdf2.GetBytes(HashSize);

            return CryptographicOperations.FixedTimeEquals(hash, computed);
        }
    }
}