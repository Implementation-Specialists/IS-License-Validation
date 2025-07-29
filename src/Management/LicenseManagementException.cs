namespace IS.LicenseValidation.Management;

public class LicenseManagementException : Exception
{
    public LicenseManagementException(LicenseManagementErrorCode errorCode)
    {
        this.ErrorCode = errorCode;
    }

    public LicenseManagementException(LicenseManagementErrorCode errorCode, string? message) : base(message)
    {
        this.ErrorCode = errorCode;
    }

    public LicenseManagementException(LicenseManagementErrorCode errorCode, string? message, Exception? innerException) : base(message, innerException)
    {
        this.ErrorCode = errorCode;
    }

    public LicenseManagementErrorCode ErrorCode { get; init; }
}
