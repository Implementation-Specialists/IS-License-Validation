using System.Runtime.Serialization;

namespace IS.LicenseValidation.Management;

public enum LicenseManagementErrorCode
{
    None = 0,
    BadRequest,
    BadLicense,
    MalformedLicense,
}
