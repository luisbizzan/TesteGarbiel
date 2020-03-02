using FWLog.Data.Models;
using FWLog.Data.Repository.CommonCtx;
using System.Linq;

namespace FWLog.Data.Repository.GeneralCtx
{
    public class NotaFiscalRecebimentoRepository : GenericRepository<NotaFiscalRecebimento>
    {
        public NotaFiscalRecebimentoRepository(Entities entities) : base(entities)
        {

        }

        public NotaFiscalRecebimento ConsultarPorCodigoIntegracao(long IdNotaFiscalRecebimento)
        {
            return Entities.NotaFiscalRecebimento.FirstOrDefault(f => f.IdNotaFiscalRecebimento == IdNotaFiscalRecebimento);
        }

        public IQueryable<NotaFiscalRecebimento> Todos()
        {
            return Entities.NotaFiscalRecebimento;
        }

        public NotaFiscalRecebimento ObterPorChave(string chaveAcesso)
        {
            return Entities.NotaFiscalRecebimento.FirstOrDefault(f => f.ChaveAcesso == chaveAcesso);
        }
    }
}
