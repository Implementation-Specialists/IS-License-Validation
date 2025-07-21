namespace IS.LicenseValidation.Management;

internal class LicenseManager(ILicenseParser licenseParser) : ILicenseManager
{
    private readonly ILicenseParser licenseParser = licenseParser ?? throw new ArgumentNullException(nameof(licenseParser));

    public bool ValidateLicense(string license, Guid tenantId, Guid productId)
    {
        if (!this.licenseParser.TryParseLicense(license, out var parsedLicense))
        {
            throw new LicenseException(LicenseErrorCode.MalformedLicense, "Failed to parse license key.");
        }

        var expirationSpan = parsedLicense.ExpirationDate - DateTimeOffset.UtcNow;

        if (expirationSpan <= TimeSpan.Zero
            || parsedLicense.TenantId != tenantId
            || parsedLicense.Product!.Id != productId)
        {
            throw new LicenseException(LicenseErrorCode.BadLicense, "License is not valid");
        }

        return true;
    }
}
