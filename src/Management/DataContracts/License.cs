namespace IS.LicenseValidation.Management.DataContracts;

public class License
{
    public DateTimeOffset ExpirationDate { get; set; }

    public Product? Product { get; set; }

    public Guid TenantId { get; set; }
}
