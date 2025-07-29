namespace IS.LicenseValidation.Management;

public class LicenseServiceOptions
{
    public string? EncryptionPassword { get; set; }

    public int NextExpirationDays { get; set; }
}
