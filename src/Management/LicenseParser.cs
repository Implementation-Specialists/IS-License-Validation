using IS.LicenseValidation.Encryption;
using IS.LicenseValidation.Management.DataContracts;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Text.Json;

namespace IS.LicenseValidation.Management;

internal class LicenseParser(ILogger<LicenseParser> logger,
    IOptionsMonitor<JsonSerializerOptions> serializerOptions,
    IOptionsMonitor<LicenseServiceOptions> serviceOptions,
    IOptionsMonitor<Encryptor> encryptor) : ILicenseParser
{
    private readonly IOptionsMonitor<JsonSerializerOptions> serializerOptions = serializerOptions ?? throw new ArgumentNullException(nameof(serializerOptions));
    private readonly IOptionsMonitor<LicenseServiceOptions> serviceOptions = serviceOptions ?? throw new ArgumentNullException(nameof(serviceOptions));
    private readonly IOptionsMonitor<Encryptor> encryptor = encryptor ?? throw new ArgumentNullException(nameof(encryptor));
    private readonly ILogger<LicenseParser> logger = logger ?? throw new ArgumentNullException(nameof(logger));

    public bool TryParseLicense(string licenseKey, [NotNullWhen(true)] out License? license)
    {
        var password = this.serviceOptions.CurrentValue.EncryptionPassword ?? throw new OptionsValidationException(typeof(LicenseServiceOptions).Name, typeof(LicenseServiceOptions), new List<string> { "Missing service options encryption password options." });

        if (!this.encryptor.CurrentValue.TryDecrypt(licenseKey, password, out var licenseBytes))
        {
            license = null;
            return false;
        }

        if (!this.TryGetJson(licenseBytes, out var licenseJson))
        {
            license = null;
            return false;
        }

        try
        {
            license = JsonSerializer.Deserialize<License>(licenseJson, this.serializerOptions.CurrentValue);

            if (license is null
                || license.TenantId == Guid.Empty
                || license.Product!.Id == Guid.Empty
                || license.ExpirationDate == default)
            {
                this.logger.LogError("Malformed license payload.");
                license = null;
                return false;
            }

            return true;
        }
        catch (JsonException ex)
        {
            this.logger.LogError(ex, "Malformed license payload.");
            license = null;
            return false;
        }
    }

    private bool TryGetJson(byte[] bytes, [NotNullWhen(true)] out string? json)
    {
        try
        {
            json = Encoding.UTF8.GetString(bytes);
            return true;
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "Malformed license.");
            json = null;
            return false;
        }
    }
}
