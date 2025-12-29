namespace Demo.ErrorHandle
{
    /// <summary>
    /// ?梁?辣?潛?靘??瘜?
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
    /// ?平?摩靘??瘜?
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
    /// 憭??靘??瘜?
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
    /// 撽?/??靘??瘜?
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
    /// ???隤支?憭?瘜?
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