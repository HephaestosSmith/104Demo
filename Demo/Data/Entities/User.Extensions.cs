using Demo.Helpers;
using System;

namespace Demo.Data.Entities
{
    public partial class User
    {
        /// <summary>
        /// 設定密碼（會產生 salt 並以 PBKDF2 儲存到 PasswordHash）
        /// </summary>
        public void SetPassword(string plainPassword)
        {
            if (plainPassword is null) throw new ArgumentNullException(nameof(plainPassword));
            PasswordHash = PasswordHelper.HashPassword(plainPassword);
        }

        /// <summary>
        /// 驗證密碼
        /// </summary>
        public bool VerifyPassword(string plainPassword)
        {
            if (plainPassword is null) throw new ArgumentNullException(nameof(plainPassword));
            return PasswordHelper.VerifyPassword(PasswordHash, plainPassword);
        }
    }
}