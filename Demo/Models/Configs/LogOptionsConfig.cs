namespace Demo.Models.Configs
{
    /// <summary>
    /// Log紀錄控制檔
    /// </summary>
    public class LogOptionsConfig
    {
        /// <summary>
        /// Log是否啟用
        /// </summary>
        public bool LogEnabled { get; set; }
        /// <summary>
        /// Request是否啟用
        /// </summary>
        public bool RequestEnabled { get; set; }

    }
}
