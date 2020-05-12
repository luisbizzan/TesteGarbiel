using FWLog.Data.Models;
using FWLog.Data.Repository.CommonCtx;
using System;
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

        public long? BuscarCodigoPeloCodigoIntegracaoVendedor(string codigoIntegracaoVendedor)
        {
            if (codigoIntegracaoVendedor == "0" || codigoIntegracaoVendedor == null)
            {
                return null;
            }

            var codRep = Convert.ToInt64(codigoIntegracaoVendedor);

            var rep = Entities.Representante.Where(f => f.CodigoIntegracaoVendedor == codRep).FirstOrDefault();

            if (rep == null)
            {
                return null;
            }
            else
            {
                return rep.IdRepresentante;
            }
        }

        public Representante BuscarPorCodigoIntegracaoVendedor(string codigoIntegracaoVendedor)
        {
            var codRep = Convert.ToInt64(codigoIntegracaoVendedor);

            return Entities.Representante.Where(f => f.CodigoIntegracaoVendedor == codRep).FirstOrDefault();
        }
    }
}
