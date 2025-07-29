using Azure.Identity;
using IS.LicenseValidation.Management;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace IS.LicenseValidation.Service;

public static class IServiceCollectionExtensions
{
    public static IServiceCollection AddJsonSerializerOptions(this IServiceCollection services)
    {
        return services
            .Configure<JsonSerializerOptions>(config =>
            {
                config.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
                config.IncludeFields = true;
                config.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                config.WriteIndented = true;
                config.Converters.Add(new JsonStringEnumConverter());
            });
    }

    public static IServiceCollection AddLicenseServiceOptions(this IServiceCollection services)
    {
        return services
            .AddOptions<LicenseServiceOptions>().Configure<IConfiguration>(
                (settings, config) =>
                {
                    config.GetSection(typeof(LicenseServiceOptions).Name).Bind(settings);
                }
            ).Services;
    }

    public static void BuildConfiguration(this IConfigurationBuilder builder)
    {
        var builtConfig = builder.Build();
        var keyVaultEndpoint = builtConfig["VaultUri"];

        if (!string.IsNullOrEmpty(keyVaultEndpoint))
        {
            // using Key Vault, either local dev or deployed
            builder.AddAzureKeyVault(new Uri(keyVaultEndpoint), new DefaultAzureCredential())
                .SetBasePath(Environment.CurrentDirectory)
                .AddJsonFile("local.settings.json", true)
                .AddEnvironmentVariables()
                .Build();
        }
        else
        {
            // local dev no Key Vault
            builder.SetBasePath(Environment.CurrentDirectory)
                .AddJsonFile("local.settings.json", true)
                .AddUserSecrets(Assembly.GetExecutingAssembly(), true)
                .AddEnvironmentVariables()
                .Build();
        }
    }
}
