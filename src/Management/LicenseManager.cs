using IS.LicenseValidation.Management.DataContracts;
using System.Diagnostics.CodeAnalysis;

namespace IS.LicenseValidation.Management;

internal class LicenseManager(ILicenseParser licenseParser) : ILicenseManager
{
    private readonly ILicenseParser licenseParser = licenseParser ?? throw new ArgumentNullException(nameof(licenseParser));

    public bool ValidateLicense(string license, Guid tenantId, Guid productId, [NotNullWhen(true)] out License? outLicense)
    {
        if (!this.licenseParser.TryParseLicense(license, out var parsedLicense))
        {
            throw new LicenseManagementException(LicenseManagementErrorCode.MalformedLicense, "Failed to parse license key.");
        }

        outLicense = parsedLicense;

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
            throw new LicenseManagementException(LicenseManagementErrorCode.ExpiredLicense, "License is expired.");
        }

        return true;
    }
}
