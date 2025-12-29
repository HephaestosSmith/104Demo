namespace Demo.ErrorHandle
{
    /// <summary>
    /// 共用元件發生例外狀況
    /// </summary>
    public class InternalException : BaseException
    {
        /// <inheritdoc />
        public InternalException(Enum errorEnum) : base(errorEnum)
        {
        }

        /// <inheritdoc />
        public InternalException(Enum errorEnum, string message) : base(errorEnum, message)
        {
        }
    }

    /// <summary>
    /// 商業邏輯例外狀況
    /// </summary>
    public class BusinessException : BaseException
    {
        /// <inheritdoc />
        public BusinessException(Enum errorEnum) : base(errorEnum)
        {
        }

        /// <inheritdoc />
        public BusinessException(Enum errorEnum, string? message) : base(errorEnum, message)
        {
        }
    }

    /// <summary>
    /// 外部服務例外狀況
    /// </summary>
    public class ExternalException : BaseException
    {
        /// <inheritdoc />
        public ExternalException(Enum errorEnum) : base(errorEnum)
        {
        }

        /// <inheritdoc />
        public ExternalException(Enum errorEnum, string? message) : base(errorEnum, message)
        {
        }
    }

    /// <summary>
    /// 驗證/授權例外狀況
    /// </summary>
    public class AuthException : BaseException
    {
        /// <inheritdoc />
        public AuthException(Enum errorEnum) : base(errorEnum)
        {
        }

        /// <inheritdoc />
        public AuthException(Enum errorEnum, string? message) : base(errorEnum, message)
        {
        }
    }

    /// <summary>
    /// 非預期錯誤例外狀況
    /// </summary>
    public class UnexpectedException : BaseException
    {
        /// <inheritdoc />
        public UnexpectedException(Enum errorEnum) : base(errorEnum)
        {
        }

        /// <inheritdoc />
        public UnexpectedException(Enum errorEnum, string? message) : base(errorEnum, message)
        {
        }
    }
}