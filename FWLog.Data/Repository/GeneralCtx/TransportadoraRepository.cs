using FWLog.Data.Models;
using FWLog.Data.Repository.CommonCtx;
using System.Linq;

namespace FWLog.Data.Repository.GeneralCtx
{
    public class TransportadoraRepository : GenericRepository<Transportadora>
    {
        public TransportadoraRepository(Entities entities) : base(entities)
        {

        }

        public Transportadora ConsultarPorCodigoIntegracao(long codigoIntegracao)
        {
            return Entities.Transportadora.FirstOrDefault(f => f.CodigoIntegracao == codigoIntegracao);
        }
    }
}
