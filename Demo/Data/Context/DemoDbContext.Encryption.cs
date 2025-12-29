using System;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Demo.Helpers;
using Demo.Data.Entities;
using System.Buffers.Text;

namespace Demo.Data.Context
{
    public partial class DemoDbContext
    {
        partial void OnModelCreatingPartial(ModelBuilder modelBuilder)
        {
            // 讀取環境變數 ENCRYPTION_KEY（原始字串或已是 Base64）
            var rawKey = Environment.GetEnvironmentVariable("ENCRYPTION_KEY");
            if (string.IsNullOrEmpty(rawKey))
            {
                throw new InvalidOperationException("ENCRYPTION_KEY not set. Please set a 32-byte key as Base64 in the environment.");
            }

            byte[] keyBytes;
            try
            {
                keyBytes = Convert.FromBase64String(rawKey);
            }
            catch (FormatException)
            {
                throw new InvalidOperationException("ENCRYPTION_KEY is not valid base64.");
            }

            if (keyBytes.Length != 32)
            {
                throw new InvalidOperationException("ENCRYPTION_KEY must be 32 bytes (base64-encoded).");
            }

            var phoneConverter = new ValueConverter<string?, string?>(
                v => v == null ? null : CryptoHelper.EncryptStringToBase64(keyBytes, v),
                v => v == null ? null : CryptoHelper.DecryptStringFromBase64(keyBytes, v)
            );

            // 設定 ValueConverter 並同步指定最大長度以避免 EF 嘗試產生過小的欄位
            modelBuilder.Entity<User>()
                .Property(e => e.Phone)
                .HasConversion(phoneConverter)
                .HasMaxLength(256);
        }

        // 將文字轉為 Base64（使用 UTF-8 編碼）
        private static string StringToBase64(string input)
        {
            if (input == null) throw new ArgumentNullException(nameof(input));
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(input));
        }
    }
}