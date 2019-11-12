using FWLog.Data.Models;
using FWLog.Data.Repository.CommonCtx;
using System.Linq;

namespace FWLog.Data.Repository.GeneralCtx
{
    public class NotaFiscalRepository : GenericRepository<NotaFiscal>
    {
        public NotaFiscalRepository(Entities entities) : base(entities) { }
               
        public NotaFiscal PegarNotaFiscal(long codigoNotaFiscal)
        {
            return Entities.NotaFiscal.FirstOrDefault(f => f.CodigoNotaFiscal == codigoNotaFiscal);
        }
              
        public NotaFiscal PegarNotaFiscalPorChave(string chaveAcesso)
        {
            return Entities.NotaFiscal.FirstOrDefault(f => f.Chave == chaveAcesso);
        }
    }
}
