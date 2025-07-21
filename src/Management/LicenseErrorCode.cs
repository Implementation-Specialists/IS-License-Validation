using System.Runtime.Serialization;

namespace IS.LicenseValidation.Management;

public enum LicenseErrorCode
{
    None = 0,
    BadRequest,
    BadLicense,
    MalformedLicense,
}
