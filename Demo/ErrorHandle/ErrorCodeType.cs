using System.ComponentModel.DataAnnotations;

namespace Demo.ErrorHandle
{
    /// <summary>
    /// 錯誤類型
    /// </summary>
    public enum ErrorCodeType
    {
        /// <summary>
        /// 系統錯誤類別
        /// </summary>
        [Display(Name = "SYS")]
        System,

        /// <summary>
        /// 驗證授權錯誤類別
        /// </summary>
        [Display(Name = "AUTH")]
        Auth,

        /// <summary>
        /// 資料格式錯誤類別
        /// </summary>
        [Display(Name = "FORMAT")]
        Format,

        /// <summary>
        /// 商業邏輯錯誤類別
        /// </summary>
        [Display(Name = "TRNS")]
        Business,
    }
}
