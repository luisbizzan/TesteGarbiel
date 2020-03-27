using FWLog.Data.Models;
using FWLog.Data.Repository.CommonCtx;

namespace FWLog.Data.Repository.GeneralCtx
{
    public class ColetorHistoricoTipoRepository : GenericRepository<ColetorHistoricoTipo>
    {
        public ColetorHistoricoTipoRepository(Entities entities) : base(entities) { }

        public void GravarHistorico(ColetorHistorico coletorHistorico)
        {
            Entities.ColetorHistorico.Add(coletorHistorico);
            Entities.SaveChanges();
        }
    }
}
