using System;
using System.Collections.Generic;
using System.Text;

namespace Demo.Models.Auth
{
    public class VerificationTokenModel
    {
        /// <summary>
        /// 顯示驗證是否成功 true:驗證成功/false:驗證失敗
        /// </summary>
        public bool IsVerification { get; set; }

        /// <summary>
        /// 驗證失敗時的錯誤訊息
        /// </summary>
        public string ErrorMsg { get; set; } = string.Empty;

        /// <summary>
        /// 驗證失敗時的錯誤碼
        /// </summary>
        public string ErrorCode { get; set; } = string.Empty;
    }
}
