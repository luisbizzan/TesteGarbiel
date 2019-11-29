using FWLog.Data.Models;
using FWLog.Data.Models.GeneralCtx;
using FWLog.Data.Repository.CommonCtx;
using System.Collections.Generic;
using System.Linq;

namespace FWLog.Data.Repository.GeneralCtx
{
    public class EmpresaRepository : GenericRepository<Empresa>
    {
        public EmpresaRepository(Entities entities) : base(entities)
        {

        }

        public IQueryable<Empresa> Todos()
        {
            return Entities.Empresa;
        }

        public long PegarPrimeiraEmpresa(string userId)
        {
            var empresa = Entities.UsuarioEmpresa.Where(w => w.Empresa.Ativo && w.UserId == userId).OrderBy(o => o.Empresa.RazaoSocial).FirstOrDefault();
            if (empresa != null)
            {
                return empresa.CompanyId;
            }

            return 0;
        }

        public IEnumerable<EmpresaSelectedItem> GetAllByUserId(string userId)
        {
            var empresas = Entities.UsuarioEmpresa.Where(w => w.Empresa.Ativo && w.UserId == userId).OrderBy(o => o.Empresa.RazaoSocial)
                .Select(s => new
                {
                    s.Empresa.Sigla,
                    s.Empresa.NomeFantasia,
                    s.Empresa.IdEmpresa
                }
                ).ToList();

            return empresas.Select(s => new EmpresaSelectedItem
            {
                Nome = string.Format("{0} - {1}", s.Sigla, s.NomeFantasia),
                IdEmpresa = s.IdEmpresa
            }).ToList();
        }

        public Empresa ConsultaPorCodigoIntegracao(int codigoIntegracao)
        {
            return Entities.Empresa.FirstOrDefault(f => f.CodigoIntegracao == codigoIntegracao);
        }
    }
}
