using FWLog.Data.Models;
using FWLog.Data.Repository.CommonCtx;
using System.Linq;

namespace FWLog.Data.Repository.GeneralCtx
{
    public class EmpresaConfigRepository : GenericRepository<EmpresaConfig>
    {
        public EmpresaConfigRepository(Entities entities) : base(entities)
        {
        }

        public EmpresaConfig ConsultarPorIdEmpresa(long idEmpresa)
        {
            return Entities.EmpresaConfig.AsNoTracking().Where(w => w.IdEmpresa == idEmpresa).FirstOrDefault();
        }

        public EmpresaConfig ConsultaPorCodigoIntegracao(long codigoIntegracao)
        {
            return Entities.EmpresaConfig.AsNoTracking().Where(w => w.Empresa.CodigoIntegracao == codigoIntegracao).FirstOrDefault();
        }

        public IQueryable<EmpresaConfig> Todos()
        {
            return Entities.EmpresaConfig;
        }
    }
}
