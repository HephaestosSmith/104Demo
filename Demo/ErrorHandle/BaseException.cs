using System.ComponentModel.DataAnnotations;

namespace Demo.ErrorHandle
{
    /// <summary>
    /// 系統例外狀況基底
    /// </summary>
    public abstract class BaseException : Exception
    {
        /// <summary>
        /// 詳細錯誤類型
        /// </summary>
        public Enum ErrorEnum { get; set; }

        /// <summary>
        /// 初始化實例。指定詳細錯誤類型
        /// </summary>
        /// <param name="errorEnum">詳細錯誤類型</param>
        public BaseException(Enum errorEnum)
        {
            ErrorEnum = errorEnum;
        }

        /// <summary>
        /// 初始化實例。指定詳細錯誤類型與錯誤訊息
        /// </summary>
        /// <param name="errorEnum">詳細錯誤類型</param>
        /// <param name="message">錯誤訊息</param>
        public BaseException(Enum errorEnum, string? message) : base(message)
        {
            ErrorEnum = errorEnum;
        }

        /// <summary>
        /// 取得詳細錯誤類型的所屬分類的顯示名稱（ErrorCodeType，如：SYS、AUTH、FORMAT、TRNS）。
        /// </summary>
        /// <returns>$"{分類名稱}.{詳細錯誤類型}"</returns>
        /// <exception cref="InternalException">詳細錯誤類型沒有標註 ErrorCatalogAttribute</exception>
        /// <exception cref="InternalException">詳細錯誤類型的所屬分類沒有標註 DisplayAttribute</exception>
        public string GetErrorCode()
        {
            var catalogAttr = ErrorEnum.GetType()
                .GetCustomAttributes(true)
                .OfType<ErrorCatalogAttribute>()
                .SingleOrDefault();

            if (catalogAttr == null)
            {
                throw new InternalException(SystemError.EnumSettingError,
                    $"{ErrorEnum.GetType().Name} need to use ErrorCatalogAttribute.");
            }

            var displayAttributes = catalogAttr.ErrorCodeType.GetType()
                .GetMember(catalogAttr.ErrorCodeType.ToString())
                .First()
                .GetCustomAttributes(true)
                .OfType<DisplayAttribute>();

            if (!displayAttributes.Any())
            {
                throw new InternalException(SystemError.EnumSettingError,
                    $"ErrorCodeType.{catalogAttr.ErrorCodeType} need to use DisplayAttribute.");
            }

            return $"{displayAttributes.First().Name}.{ErrorEnum}";
        }
    }

    /// <summary>
    /// 標示錯誤類別
    /// </summary>
    [AttributeUsage(AttributeTargets.Enum, AllowMultiple = false)]
    public class ErrorCatalogAttribute : Attribute
    {
        /// <summary> 錯誤類別 </summary>
        public ErrorCodeType ErrorCodeType { get; set; }

        /// <summary>
        /// 初始化 ErrorCatalogAttribute 類別的實例。指定錯誤類別
        /// </summary>
        /// <param name="errorCodeType">錯誤類別</param>
        public ErrorCatalogAttribute(ErrorCodeType errorCodeType)
        {
            ErrorCodeType = errorCodeType;
        }
    }
}
