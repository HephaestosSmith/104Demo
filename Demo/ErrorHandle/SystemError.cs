namespace Demo.ErrorHandle
{
    /// <summary>
    /// 系統相關錯誤
    /// </summary>
    [ErrorCatalog(ErrorCodeType.System)]
    public enum SystemError
    {
        /// <summary> 未知錯誤 </summary>
        UnknowError,

        /// <summary> Enum 錯誤 </summary>
        EnumSettingError,

        /// <summary> Azure 錯誤 </summary>
        AzureError,

        // <summary> Redis 錯誤 </summary>
        RedisError,
    }
}
