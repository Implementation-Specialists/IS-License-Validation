namespace IS.LicenseValidation.Management.DataContracts.Responses;

public class ErrorResponse
{
    public ErrorCode Code { get; set; }

    public string Message { get; set; } = string.Empty;
}
