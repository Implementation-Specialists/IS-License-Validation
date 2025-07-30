namespace IS.LicenseValidation.Management.DataContracts.Responses;

public class LicenseValidationResponseV1 : ErrorResponse
{
    public License? License { get; set; }

    public bool IsValid { get; set; }

    public DateTimeOffset NextValidationDate { get; set; }
}