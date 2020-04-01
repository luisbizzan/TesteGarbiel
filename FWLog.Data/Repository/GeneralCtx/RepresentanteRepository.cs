using FWLog.Data.Models;
using FWLog.Data.Repository.CommonCtx;
using System.Linq;


namespace FWLog.Data.Repository.GeneralCtx
{
    public class RepresentanteRepository : GenericRepository<Representante>
    {
        public RepresentanteRepository(Entities entities) : base(entities) { }

        public Representante ConsultarPorCodigoIntegracao(long codigoIntegracao)
        {
            return Entities.Representante.FirstOrDefault(f => f.CodigoIntegracao == codigoIntegracao);
        }

        public long BuscarCodigoPeloCodigoIntegracaoVendedor(long codigoIntegracaoVendedor)
        {
            return Entities.Representante.Where(f => f.CodigoIntegracaoVendedor == codigoIntegracaoVendedor).Select(x => x.IdRepresentante).FirstOrDefault();
        }
    }
}
