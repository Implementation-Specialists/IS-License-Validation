using System.Runtime.Serialization;

namespace IS.LicenseValidation.Management.DataContracts.Responses;

public enum ErrorCode
{
    None = 0,
    BadRequest,
    BadLicense,
    MalformedLicense,
    InternalError,
}
