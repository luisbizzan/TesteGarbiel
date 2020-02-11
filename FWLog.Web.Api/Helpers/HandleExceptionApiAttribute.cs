using FWLog.Web.Api.GlobalResources.General;
using FWLog.Web.Api.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Http.Filters;

namespace FWLog.Web.Api.Helpers
{
    public class HandleExceptionApiAttribute : ExceptionFilterAttribute
    {
        public override void OnException(HttpActionExecutedContext context)
        {
            var httpResponse = new HttpResponseMessage();
            var apiError = new ApiError();

            if (context.Exception is HttpException httpException)
            {
                httpResponse.StatusCode = (HttpStatusCode)httpException.GetHttpCode();
                apiError.Message = httpException.Message;
            }
            else
            {
                httpResponse.StatusCode = HttpStatusCode.InternalServerError;
                apiError.Message = GeneralStrings.InternalServerError;
            }

            var apiErrorResponse = new ApiErrorModelResponse
            {
                Errors = new List<ApiError> { apiError }
            };

            httpResponse.Content = new StringContent(JsonConvert.SerializeObject(apiErrorResponse));
            httpResponse.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            context.Response = httpResponse;

            if (IsServerError(httpResponse.StatusCode.GetHashCode()))
            {
                LogHelper.Error(context.Exception);
            }
        }

        private bool IsServerError(int statusCode)
        {
            return statusCode >= 500 && statusCode < 600;
        }
    }
}