using IS.LicenseValidation.Management.DataContracts;
using System.Diagnostics.CodeAnalysis;

namespace IS.LicenseValidation.Management;

public interface ILicenseManager
{
    License ValidateLicense(string license, Guid tenantId, Guid productId);

}
