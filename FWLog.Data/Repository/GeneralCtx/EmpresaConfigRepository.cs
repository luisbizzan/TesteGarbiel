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
            return Entities.EmpresaConfig.Where(w => w.IdEmpresa == idEmpresa).FirstOrDefault();
        }

        public IQueryable<EmpresaConfig> Todos()
        {
            return Entities.EmpresaConfig;
        }
    }
}
