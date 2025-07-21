using IS.LicenseValidation.Management.DataContracts;

namespace IS.LicenseValidation.Management;

public interface ILicenseManager
{
    bool ValidateLicense(string license, Guid tenantId, Guid productId);

}
