using FWLog.Web.Api.GlobalResources.General;
using FWLog.Web.Api.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Http.Filters;
using System.Web.Routing;

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

            // Validação colocada porque erros diferente de 5xx não devem ser logados.      
            if (IsServerError(httpResponse.StatusCode.GetHashCode()))
            {
                LogHelper.Error(context.Exception);
            }
        }

        private void HandleException(Exception exception)
        {
            var routeData = new RouteData();

            if (exception is HttpException httpException)
            {
                HttpContext.Current.Response.StatusCode = httpException.GetHttpCode();
            }
            else
            {
                HttpContext.Current.Response.StatusCode = 500;
            }
        }

        private bool IsServerError(int statusCode)
        {
            return statusCode >= 500 && statusCode < 600;
        }

    }
}