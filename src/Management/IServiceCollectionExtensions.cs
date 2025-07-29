using IS.LicenseValidation.Encryption;
using Microsoft.Extensions.DependencyInjection;

namespace IS.LicenseValidation.Management;

public static class IServiceCollectionExtensions
{
    public static IServiceCollection AddLicenseManagement(this IServiceCollection services)
    {
        return services
            .AddScoped<ILicenseParser, LicenseParser>()
            .AddScoped<ILicenseManager, LicenseManager>()
            .AddScoped<ILicenseManagement, LicenseManagement>();
    }

    public static IServiceCollection AddEncryption(this IServiceCollection services)
    {
        return services
            .AddScoped<IEncryptor, Encryptor>();
    }

}
