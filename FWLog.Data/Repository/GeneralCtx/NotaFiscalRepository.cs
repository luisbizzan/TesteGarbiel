using FWLog.Data.Models;
using FWLog.Data.Repository.CommonCtx;
using System.Linq;

namespace FWLog.Data.Repository.GeneralCtx
{
    public class NotaFiscalRepository : GenericRepository<NotaFiscal>
    {
        public NotaFiscalRepository(Entities entities) : base(entities) { }
               
        public NotaFiscal PegarNotaFiscal(long codigoIntegracao)
        {
            return Entities.NotaFiscal.FirstOrDefault(f => f.CodigoIntegracao == codigoIntegracao);
        }
              
        public NotaFiscal PegarNotaFiscalPorChave(string chaveAcesso)
        {
            return Entities.NotaFiscal.FirstOrDefault(f => f.ChaveAcesso == chaveAcesso);
        }
    }
}
