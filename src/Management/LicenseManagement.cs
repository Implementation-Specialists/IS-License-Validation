namespace IS.LicenseValidation.Management;

internal class LicenseManagement(ILicenseManager licenseManager) : ILicenseManagement
{
    public ILicenseManager LicenseManager => licenseManager;
}
