using IS.LicenseValidation.Management.DataContracts;
using System.Diagnostics.CodeAnalysis;

namespace IS.LicenseValidation.Management;

public interface ILicenseManager
{
    bool ValidateLicense(string license, Guid tenantId, Guid productId, [NotNullWhen(true)] out License? outLicense);

}
