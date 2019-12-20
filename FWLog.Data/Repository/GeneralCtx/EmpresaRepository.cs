using FWLog.Data.Models;
using FWLog.Data.Models.GeneralCtx;
using FWLog.Data.Repository.CommonCtx;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace FWLog.Data.Repository.GeneralCtx
{
    public class EmpresaRepository : GenericRepository<Empresa>
    {
        public EmpresaRepository(Entities entities) : base(entities)
        {

        }

        public IQueryable<Empresa> Tabela()
        {
            return Entities.Empresa;
        }

        public long RetornarEmpresaPrincipal(string userId)
        {
            PerfilUsuario perfilUsuario = Entities.PerfilUsuario.Where(w => w.UsuarioId == userId).FirstOrDefault();

            if (perfilUsuario.EmpresaId.HasValue)
            {
                return perfilUsuario.EmpresaId.Value;
            }

            UsuarioEmpresa usuarioEmpresa = Entities.UsuarioEmpresa.Where(w => w.UserId.Equals(userId)).FirstOrDefault();

            if (usuarioEmpresa == null)
            {
                return 0;
            }

            return usuarioEmpresa.IdEmpresa;
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
            });
        }

        public ReadOnlyCollection<long> IdEmpresasPorUsuario(string idUsuario)
        {
            return Entities.UsuarioEmpresa.AsNoTracking().Where(w => w.Empresa.Ativo && w.UserId == idUsuario)
                .Select(s => s.Empresa.IdEmpresa).Distinct().ToList().AsReadOnly();
        }

        public Empresa ConsultaPorCodigoIntegracao(int codigoIntegracao)
        {
            return Entities.Empresa.FirstOrDefault(f => f.CodigoIntegracao == codigoIntegracao);
        }
    }
}
