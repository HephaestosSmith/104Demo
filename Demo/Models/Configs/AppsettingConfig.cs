namespace Demo.Models.Configs
{

    /// <summary>
    /// 專案設定檔
    /// </summary>
    public class AppsettingConfig
    {
        /// <summary>
        /// Log啟用設定
        /// </summary>
        public LogOptionsConfig? LogOptions { get; set; } = null;

        /// <summary>
        /// JWT 設定檔
        /// </summary>
        public JwtConfig? JWT { get; set; } = null;

    }
}
