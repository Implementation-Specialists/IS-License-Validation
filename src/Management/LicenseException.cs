namespace IS.LicenseValidation.Management;

public class LicenseException : Exception
{
    public LicenseException(LicenseErrorCode errorCode)
    {
        this.ErrorCode = errorCode;
    }

    public LicenseException(LicenseErrorCode errorCode, string? message) : base(message)
    {
        this.ErrorCode = errorCode;
    }

    public LicenseException(LicenseErrorCode errorCode, string? message, Exception? innerException) : base(message, innerException)
    {
        this.ErrorCode = errorCode;
    }

    public LicenseErrorCode ErrorCode { get; init; }
}
