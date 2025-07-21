using IS.LicenseValidation.Management.DataContracts;
using System.Diagnostics.CodeAnalysis;

namespace IS.LicenseValidation.Management;

 internal interface ILicenseParser
{
    bool TryParseLicense(string licenseKey, [NotNullWhen(true)] out License? license);
}
