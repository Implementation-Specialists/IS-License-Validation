using IS.LicenseValidation.Management.DataContracts;
using System.Diagnostics.CodeAnalysis;

namespace IS.LicenseValidation.Management;

internal class LicenseManager(ILicenseParser licenseParser) : ILicenseManager
{
    private readonly ILicenseParser licenseParser = licenseParser ?? throw new ArgumentNullException(nameof(licenseParser));

    public License ValidateLicense(string license, Guid tenantId, Guid productId)
    {
        License? parsedLicense;

        try
        {
            if (!this.licenseParser.TryParseLicense(license, out parsedLicense))
            {
                throw new LicenseManagementException(LicenseManagementErrorCode.MalformedLicense, "Failed to parse license key.");
            }
        }
        catch (Exception ex)
        {
            throw new LicenseManagementException(LicenseManagementErrorCode.MalformedLicense, "Failed to parse license key.", ex);
        }

        

        if (parsedLicense.TenantId != tenantId)
        {
            throw new LicenseManagementException(LicenseManagementErrorCode.BadLicense, "License is not valid for tenant.");
        }

        if (parsedLicense.Product.Id != productId)
        {
            throw new LicenseManagementException(LicenseManagementErrorCode.BadLicense, "License is not valid for product.");
        }

        return parsedLicense;
    }
}
