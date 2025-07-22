using IS.LicenseValidation.Management;
using IS.LicenseValidation.Service;
using IS.LicenseValidation.Service.Middleware;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication(builder =>
    {
        builder
        .UseMiddleware<LicenseManagementExceptionHandler>()
            .UseDefaultWorkerMiddleware();
    })
    .ConfigureAppConfiguration((context, builder) =>
    {
        builder.BuildConfiguration();
    })
    .ConfigureServices(services =>
    {
        services
            .AddApplicationInsightsTelemetryWorkerService()
            .ConfigureFunctionsApplicationInsights()
            .AddSingleton<LicenseManagementExceptionHandler>()
            .AddEncryption()
            .AddJsonSerializerOptions()
            .AddLicenseServiceOptions()
            .AddLicenseManagement();
    })
    .Build();

host.Run();
