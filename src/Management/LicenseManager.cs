namespace IS.LicenseValidation.Management;

internal class LicenseManager(ILicenseParser licenseParser) : ILicenseManager
{
    private readonly ILicenseParser licenseParser = licenseParser ?? throw new ArgumentNullException(nameof(licenseParser));

    public bool ValidateLicense(string license, Guid tenantId, Guid productId)
    {
        if (!this.licenseParser.TryParseLicense(license, out var parsedLicense))
        {
            throw new LicenseManagementException(LicenseManagementErrorCode.MalformedLicense, "Failed to parse license key.");
        }

        if (parsedLicense.TenantId != tenantId)
        {
            throw new LicenseManagementException(LicenseManagementErrorCode.BadLicense, "License is not valid for tenant.");
        }

        if (parsedLicense.Product!.Id != productId)
        {
            throw new LicenseManagementException(LicenseManagementErrorCode.BadLicense, "License is not valid for product.");
        }

        var expirationSpan = parsedLicense.ExpirationDate - DateTimeOffset.UtcNow;

        if (expirationSpan <= TimeSpan.Zero)
        {
            throw new LicenseManagementException(LicenseManagementErrorCode.BadLicense, "License is expired.");
        }

        return true;
    }
}
