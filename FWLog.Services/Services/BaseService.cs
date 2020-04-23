using DartDigital.Library.Exceptions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace FWLog.Services.Services
{
    public class BaseService
    {
        public const string SIGLA_EMPRESA_MATRIZ = "00";
        public const string SIGLA_EMPRESA_MANAUS = "A1";
        public const string SIGLA_EMPRESA_MINAS = "BH";

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

        protected void ValidaELancaExcecaoIntegridade(Exception exception)
        {
            if (exception.InnerException != null &&
                exception.InnerException is System.Data.Entity.Core.UpdateException &&
                exception.InnerException.InnerException != null &&
                exception.InnerException.InnerException is Oracle.ManagedDataAccess.Client.OracleException &&
                exception.InnerException.InnerException.Message.Contains("restrição de integridade"))
            {

                throw new BusinessException("O registro não pode ser apagado pois possui vínculos.");
            }
        }
    }
}