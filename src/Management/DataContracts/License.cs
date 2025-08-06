namespace IS.LicenseValidation.Management.DataContracts;

public class License
{
    public DateTimeOffset IssueDate { get; set; }

    public DateTimeOffset ExpirationDate { get; set; }

    public Product Product { get; set; } = new Product();

    public Guid TenantId { get; set; }
}
