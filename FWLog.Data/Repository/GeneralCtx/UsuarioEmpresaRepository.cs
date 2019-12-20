using FWLog.Data.Models;
using FWLog.Data.Models.DataTablesCtx;
using FWLog.Data.Models.FilterCtx;
using FWLog.Data.Repository.CommonCtx;
using System.Collections.Generic;
using System.Linq;

namespace FWLog.Data.Repository.GeneralCtx
{
    public class UsuarioEmpresaRepository : GenericRepository<UsuarioEmpresa>
    {
        public UsuarioEmpresaRepository(Entities entities) : base(entities)
        {

        }

        public List<UsuarioEmpresa> ObterPorEmpresa(long idEmpresa)
        {
            return Entities.UsuarioEmpresa.Where(x => x.IdEmpresa == idEmpresa).ToList();
        }

        public List<long> GetAllEmpresasByUserId(string userId)
        {
            return Entities.UsuarioEmpresa.Where(w => w.UserId == userId).Select(s => s.IdEmpresa).ToList();
        }

        public void DeleteByUserId(string userId, long idEmpresa)
        {
            var rel = Entities.UsuarioEmpresa.Where(w => w.UserId == userId && w.IdEmpresa == idEmpresa).FirstOrDefault();

            if (rel != null)
            {
                Entities.UsuarioEmpresa.Remove(rel);
            }
        }

        public IQueryable<UsuarioEmpresa> Tabela()
        {
            return Entities.UsuarioEmpresa;
        }
    }
}
