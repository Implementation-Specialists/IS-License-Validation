using IS.LicenseValidation.Management;
using IS.LicenseValidation.Management.DataContracts.Requests;
using IS.LicenseValidation.Management.DataContracts.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace IS.LicenseValidation.Service.Functions;

public class ValidateLicense(ILogger<ValidateLicense> logger, ILicenseManagement licenseManagement, IOptionsMonitor<JsonSerializerOptions> serializerOptions, IOptionsMonitor<LicenseServiceOptions> serviceOptions)
{
    private readonly ILogger<ValidateLicense> logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private readonly ILicenseManagement licenseManagement = licenseManagement ?? throw new ArgumentNullException(nameof(licenseManagement));
    private readonly IOptionsMonitor<JsonSerializerOptions> serializerOptions = serializerOptions ?? throw new ArgumentNullException(nameof(serializerOptions));
    private readonly IOptionsMonitor<LicenseServiceOptions> serviceOptions = serviceOptions ?? throw new ArgumentNullException(nameof(serviceOptions));

    [Function("ValidateLicense")]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function,
        "post",
        Route = "v1/validateLicense/{tenantId:minlength(36)}/{productId:minlength(36)}")] HttpRequest req,
        Guid tenantId,
        Guid productId)
    {
        logger.LogInformation($"Validating license for tenant {tenantId} and product {productId}");

        try
        {
            var validationRequest = await JsonSerializer.DeserializeAsync<LicenseValidationRequestV1>(req.Body, serializerOptions.CurrentValue);
            licenseManagement.LicenseManager.ValidateLicense(validationRequest!.License!, tenantId, productId);
        }
        catch (Exception ex)
        {
            throw new LicenseException(LicenseErrorCode.BadRequest, "Invalid license validation request body.", ex);
        }

        var validationResponse = new LicenseValidationResponseV1()
        {
            IsValid = true,
            NextValidationDate = DateTimeOffset.UtcNow.AddDays(this.serviceOptions.CurrentValue.NextExpirationDays),
        };

        logger.LogInformation($"Successfully validated license for tenant {tenantId} and product {productId}");

        return new OkObjectResult(validationResponse);
    }
}