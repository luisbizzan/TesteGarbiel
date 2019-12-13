using FWLog.Data.Models;
using FWLog.Data.Models.DataTablesCtx;
using FWLog.Data.Models.FilterCtx;
using FWLog.Data.Repository.CommonCtx;
using System.Collections.Generic;
using System.Linq;

namespace FWLog.Data.Repository.GeneralCtx
{
    public class PerfilUsuarioRepository : GenericRepository<PerfilUsuario>
    {
        public PerfilUsuarioRepository(Entities entities) : base(entities)
        {
        }

        public PerfilUsuario GetByUserId(string userId)
        {
            return Entities.PerfilUsuario.FirstOrDefault(f => f.UsuarioId == userId);
        }

        public IQueryable<PerfilUsuario> Tabela()
        {
            return Entities.PerfilUsuario;
        }

        public List<UsuarioListaLinhaTabela> PesquisarLista(DataTableFilter<UsuarioListaFiltro> model)
        {
            IQueryable<UsuarioListaLinhaTabela> query =
                Entities.PerfilUsuario.AsNoTracking().Where(w => w.UsuarioEmpresas.Any(c => c.IdEmpresa == model.CustomFilter.IdEmpresa) &&
                    (model.CustomFilter.UserName.Equals(string.Empty) || w.Usuario.UserName.Contains(model.CustomFilter.UserName)) &&
                    (model.CustomFilter.Nome.Equals(string.Empty) || w.Nome.Contains(model.CustomFilter.Nome)) &&
                    (model.CustomFilter.Email.Equals(string.Empty) || w.Usuario.Email.Contains(model.CustomFilter.Email)) &&
                    (model.CustomFilter.Ativo.HasValue == false || w.Ativo == model.CustomFilter.Ativo.Value))
                .Select(s => new UsuarioListaLinhaTabela
                {
                    UserName = s.Usuario.UserName,
                    Email = s.Usuario.Email,
                    Nome = s.Nome,
                    Status = s.Ativo ? "Ativo" : "Inativo"
                })
                .OrderBy(model.OrderByColumn, model.OrderByDirection)
                .Skip(model.Start)
                .Take(model.Length);

            return query.ToList();
        }
    }
}
