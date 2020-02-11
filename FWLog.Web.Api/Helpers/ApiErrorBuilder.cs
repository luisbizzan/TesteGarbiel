using FWLog.Web.Api.Models;
using System;
using System.Collections.Generic;
using System.Web.Http.ModelBinding;

namespace FWLog.Web.Api.Helpers
{
    public class ApiErrorBuilder
    {
        public static ApiErrorModelResponse BuildResponse(string message)
        {
            var apiError = BuildError(message);

            var apiErrorResponse = new ApiErrorModelResponse
            {
                Errors = new List<ApiError> { apiError }
            };

            return apiErrorResponse;
        }

        public static ApiErrorModelResponse BuildResponse(ModelStateDictionary modelState)
        {
            var apiErrorResponse = new ApiErrorModelResponse();

            foreach (ModelState value in modelState.Values)
            {
                foreach (ModelError error in value.Errors)
                {
                    var apiError = new ApiError
                    {
                        Message = error.ErrorMessage.NullOrEmpty() ? string.Format("Um campo obrigatório não foi informado ou não está no padrão correto, favor verificar o modelo. A exceção gerada foi: {0}.", error.Exception) : error.ErrorMessage
                    };

                    apiErrorResponse.Errors.Add(apiError);
                }
            }

            return apiErrorResponse;
        }

        private static ApiError BuildError(string message)
        {
            var apiError = new ApiError
            {
                Message = message
            };

            return apiError;
        }
    }
}