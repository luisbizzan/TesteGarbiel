using System.Collections.Generic;
using System.Text;

namespace FWLog.Web.Api.Models
{
    public class ApiErrorModelResponse
    {
        public ApiErrorModelResponse()
        {
            Errors = new List<ApiError>();
        }

        public List<ApiError> Errors { get; set; }

        public override string ToString()
        {
            if (Errors == null || Errors.Count == 0)
            {
                return base.ToString();
            }
            else
            {
                StringBuilder toReturn = new StringBuilder();

                foreach (var erro in Errors)
                {
                    toReturn.Append(string.Concat(erro.Message, " - "));
                }

                return toReturn.ToString();
            }
        }
    }

    public class ApiError
    {
        public string Message { get; set; }
    }
}