namespace IS.LicenseValidation.Management.DataContracts.Responses;

public class LicenseValidationResponseV1 : ErrorResponse
{
    public bool IsValid { get; set; }

    public DateTimeOffset NextValidationDate { get; set; }
}