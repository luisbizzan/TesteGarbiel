using System.Threading.Tasks;
using FWLog.Services.Integracao;
using FWLog.Services.Model;
using System.Collections.Generic;

namespace FWLog.Services.Services
{
    public class NotaFiscalService
    {
        public async Task ConsultaNotaFiscalCompra()
        {
            List<NotaFiscalIntegracao> notasInt = await IntegracaoSankhya.Instance.PreExecuteQuery<NotaFiscalIntegracao>();


        }
    }
}
