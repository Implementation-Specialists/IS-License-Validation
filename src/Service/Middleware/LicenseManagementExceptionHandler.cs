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
                await HandleLicenseManagementException(context, licenseManagementException);
            }
            else
            {
                await HandleException(context, ex);
            }   
        }
    }

    private async Task HandleException(FunctionContext context, Exception exception)
    {
        logger.LogError(exception.Message, exception);

        var request = await context.GetHttpRequestDataAsync();

        if (request != null)
        {
            var response = request.CreateResponse();
            response.StatusCode = HttpStatusCode.InternalServerError;

            await response.WriteAsJsonAsync(new BaseResponse
            {
                Error = new ErrorResponse
                {
                    Code = ErrorCode.InternalError,
                    Message = "An internal server error occurred."
                }
            });

            context.GetInvocationResult().Value = response;
        }
    }

    private async Task HandleLicenseManagementException(FunctionContext context, LicenseManagementException exception)
    {
        logger.LogError(exception.Message, exception);

        var request = await context.GetHttpRequestDataAsync();

        if (request != null)
        {
            var response = request.CreateResponse();

            response.StatusCode = exception.ErrorCode switch
            {
                LicenseManagementErrorCode.BadLicense => HttpStatusCode.NotFound,
                LicenseManagementErrorCode.MalformedLicense => HttpStatusCode.UnprocessableContent,
                LicenseManagementErrorCode.BadRequest or LicenseManagementErrorCode.ExpiredLicense => HttpStatusCode.BadRequest,
                _ => HttpStatusCode.InternalServerError,
            };

            await response.WriteAsJsonAsync(new BaseResponse
            {
                Error = new ErrorResponse
                {
                    Code = exception.ErrorCode switch
                    {
                        LicenseManagementErrorCode.BadLicense => ErrorCode.BadLicense,
                        LicenseManagementErrorCode.MalformedLicense => ErrorCode.MalformedLicense,
                        LicenseManagementErrorCode.BadRequest => ErrorCode.BadRequest,
                        LicenseManagementErrorCode.ExpiredLicense => ErrorCode.ExpiredLicense,
                        _ => ErrorCode.InternalError,
                    },
                    Message = exception.ErrorCode switch
                    {
                        LicenseManagementErrorCode.MalformedLicense => "Not a valid license.",
                        LicenseManagementErrorCode.ExpiredLicense => "License is expired.",
                        LicenseManagementErrorCode.BadLicense => "License is not valid for tenant or product.",
                        LicenseManagementErrorCode.BadRequest => "Invalid license request.",
                        _ => "An internal server error occurred.",
                    }
                }
            });

            context.GetInvocationResult().Value = response;
        }
    }
}
