using IS.LicenseValidation.Management;
using IS.LicenseValidation.Management.DataContracts.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.Functions.Worker.Middleware;
using Microsoft.Extensions.Logging;
using System.Net;

namespace IS.LicenseValidation.Service.Middleware;

internal class LicenseExceptionHandler(ILogger<LicenseExceptionHandler> logger) : IFunctionsWorkerMiddleware
{
    private readonly ILogger<LicenseExceptionHandler> logger = logger ?? throw new ArgumentNullException(nameof(logger));

    public async Task Invoke(FunctionContext context, FunctionExecutionDelegate next)
    {
        try
        {
            await next.Invoke(context);
        }
        catch (Exception ex)
        {
            if (ex is LicenseException licenseException) 
            {
                logger.LogError(ex.Message);

                var request = await context.GetHttpRequestDataAsync();

                if (request != null)
                {
                    var response = request.CreateResponse();
                    response.StatusCode = licenseException.ErrorCode switch
                    {
                        LicenseErrorCode.BadLicense => HttpStatusCode.NotFound,
                        LicenseErrorCode.MalformedLicense => HttpStatusCode.UnprocessableContent,
                        LicenseErrorCode.BadRequest => HttpStatusCode.BadRequest,
                        _ => HttpStatusCode.InternalServerError,
                    };

                    await response.WriteAsJsonAsync(new ErrorResponse
                    {
                        ErrorCode = licenseException.ErrorCode switch
                        {
                            LicenseErrorCode.BadLicense => ErrorCode.BadLicense,
                            LicenseErrorCode.MalformedLicense => ErrorCode.MalformedLicense,
                            LicenseErrorCode.BadRequest => ErrorCode.BadRequest,
                            _ => ErrorCode.InternalError,
                        },
                        Message = licenseException.ErrorCode switch
                        {
                            LicenseErrorCode.MalformedLicense => "Request contained a malformed license.",
                            LicenseErrorCode.BadLicense => "License is invalid.",
                            LicenseErrorCode.BadRequest => "License request is invalid.",
                            _ => "An internal server error occurred.",
                        }
                    });

                    context.GetInvocationResult().Value = response;
                }
            }            
        }
    }
}
