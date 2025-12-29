using Demo.Models.Configs;

namespace Demo.Settings
{
    /// <summary>
    /// 專案設定檔物件化
    /// </summary>
    public static class AppsettingSetting
    {
        /// <summary>
        /// 宣告靜態介面
        /// </summary>
        private static readonly IConfiguration? _config;

        /// <summary>
        /// IConfiguration 介面
        /// </summary>
        /// <returns>_config</returns>
        public static IConfiguration? GetConfigServiceInstance()
        {
            return _config;
        }
        /// <summary>
        /// 取得Appsettings 設定檔
        /// </summary>
        /// <param name="config">config</param>
        /// <param name="webHostEnvironment">webHostEnvironment</param>
        /// <returns>AppsettingConfig</returns>
        /// 
        public static AppsettingConfig GetAppsettingConfig(IConfiguration config)
        {

            var appsettingConfig = new AppsettingConfig();

            //Mapping LogOptions
            appsettingConfig.LogOptions = config.GetSection("LogOptions").Get<LogOptionsConfig>();

            //Mapping JWT
            appsettingConfig.JWT = config.GetSection("JWT").Get<JwtConfig>();

            return appsettingConfig;

        }

    }
}
