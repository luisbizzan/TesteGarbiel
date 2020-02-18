using FWLog.Web.Api.Models;
using System;
using System.Collections.Generic;
using System.Web.Http.ModelBinding;

namespace FWLog.Web.Api.Helpers
{
    public class ApiErrorBuilder
    {
        public static ApiErroResposta BuildResponse(string message)
        {
            var apiError = BuildError(message);

            var apiErrorResponse = new ApiErroResposta
            {
                Erros = new List<ApiErro> { apiError }
            };

            return apiErrorResponse;
        }

        public static ApiErroResposta BuildResponse(ModelStateDictionary modelState)
        {
            var apiErrorResponse = new ApiErroResposta();

            foreach (ModelState value in modelState.Values)
            {
                foreach (ModelError error in value.Errors)
                {
                    var apiError = new ApiErro
                    {
                        Mensagem = error.ErrorMessage.NullOrEmpty() ? string.Format("Um campo obrigatório não foi informado ou não está no padrão correto, favor verificar o modelo. A exceção gerada foi: {0}.", error.Exception) : error.ErrorMessage
                    };

                    apiErrorResponse.Erros.Add(apiError);
                }
            }

            return apiErrorResponse;
        }

        private static ApiErro BuildError(string message)
        {
            var apiError = new ApiErro
            {
                Mensagem = message
            };

            return apiError;
        }
    }
}