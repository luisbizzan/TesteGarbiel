using System;

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
    }
}
