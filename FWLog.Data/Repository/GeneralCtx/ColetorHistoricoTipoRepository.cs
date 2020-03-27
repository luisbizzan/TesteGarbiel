using FWLog.Data.Models;
using FWLog.Data.Repository.CommonCtx;
using System.Collections.Generic;

namespace FWLog.Data.Repository.GeneralCtx
{
    public class ColetorHistoricoTipoRepository : GenericRepository<ColetorHistoricoTipo>
    {
        public ColetorHistoricoTipoRepository(Entities entities) : base(entities) { }

        public IEnumerable<ColetorHistoricoTipo> Todos()
        {
            return Entities.ColetorHistoricoTipo;
        }

        public void GravarHistorico(ColetorHistorico coletorHistorico)
        {
            Entities.ColetorHistorico.Add(coletorHistorico);
            Entities.SaveChanges();
        }
    }
}
