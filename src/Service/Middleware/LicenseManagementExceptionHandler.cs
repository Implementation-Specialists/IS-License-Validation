using IS.LicenseValidation.Management;
using IS.LicenseValidation.Management.DataContracts.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.Functions.Worker.Middleware;
using Microsoft.Extensions.Logging;
using System.Net;

namespace IS.LicenseValidation.Service.Middleware;

internal class LicenseManagementExceptionHandler(ILogger<LicenseManagementExceptionHandler> logger) : IFunctionsWorkerMiddleware
{
    private readonly ILogger<LicenseManagementExceptionHandler> logger = logger ?? throw new ArgumentNullException(nameof(logger));

    public async Task Invoke(FunctionContext context, FunctionExecutionDelegate next)
    {
        try
        {
            await next.Invoke(context);
        }
        catch (Exception ex)
        {
            if (ex is LicenseManagementException licenseManagementException) 
            {
                logger.LogError(ex.Message);

                var request = await context.GetHttpRequestDataAsync();

                if (request != null)
                {
                    var response = request.CreateResponse();
                    response.StatusCode = licenseManagementException.ErrorCode switch
                    {
                        LicenseManagementErrorCode.BadLicense => HttpStatusCode.NotFound,
                        LicenseManagementErrorCode.MalformedLicense => HttpStatusCode.UnprocessableContent,
                        LicenseManagementErrorCode.BadRequest => HttpStatusCode.BadRequest,
                        _ => HttpStatusCode.InternalServerError,
                    };

                    await response.WriteAsJsonAsync(new ErrorResponse
                    {
                        ErrorCode = licenseManagementException.ErrorCode switch
                        {
                            LicenseManagementErrorCode.BadLicense => ErrorCode.BadLicense,
                            LicenseManagementErrorCode.MalformedLicense => ErrorCode.MalformedLicense,
                            LicenseManagementErrorCode.BadRequest => ErrorCode.BadRequest,
                            _ => ErrorCode.InternalError,
                        },
                        Message = licenseManagementException.ErrorCode switch
                        {
                            LicenseManagementErrorCode.MalformedLicense => "Request contained a malformed license.",
                            LicenseManagementErrorCode.BadLicense => "License is invalid.",
                            LicenseManagementErrorCode.BadRequest => "License request is invalid.",
                            _ => "An internal server error occurred.",
                        }
                    });

                    context.GetInvocationResult().Value = response;
                }
            }            
        }
    }
}
