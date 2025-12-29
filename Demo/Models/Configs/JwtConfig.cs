using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.Models.Configs
{
    /// <summary>
    /// JWT 設定檔
    /// </summary>
    public class JwtConfig
    {


        /// <summary>
        /// Secret
        /// </summary>
        public string Secret { get; set; } = string.Empty;

        /// <summary>
        /// Issuer
        /// </summary>
        public string Issuer { get; set; } = string.Empty;

        /// <summary>
        /// Audience
        /// </summary>
        public string Audience { get; set; } = string.Empty;    

        /// <summary>
        /// 共通有效期限(分)
        /// </summary>
        public string Expires { set; get; } = string.Empty;

        /// <summary>
        /// WEB 有效期限(分)
        /// </summary>
        public string WEBExpires = string.Empty;


        /// <summary>
        /// APP 有效期限(分)
        /// </summary>
        public string APPExpires = string.Empty;


    }
}
