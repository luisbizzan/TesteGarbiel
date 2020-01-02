using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace FWLog.Services.Services
{
    public class BaseService
    {
        public void ValidarCampo(string campo, string nome)
        {
            if (campo == null)
            {
                throw new NullReferenceException(nome);
            }
        }
       
        public void ValidarDadosIntegracao<T>(T objtIntegracao)
        {
            var context = new ValidationContext(objtIntegracao, null, null);
            var results = new List<ValidationResult>();
            if (!(Validator.TryValidateObject(objtIntegracao, context, results, true)))            
            {
                if (!results.NullOrEmpty())
                {
                    throw new Exception(string.Join(" ", results.Select(s => s.ErrorMessage).ToArray()));
                }
            }
        }
    }
}
